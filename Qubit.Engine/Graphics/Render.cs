using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Qubit.Engine.Core;
using Silk.NET.Assimp;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D.Compilers;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;
using Silk.NET.Maths;

namespace Qubit.Engine.Graphics
{
    public struct Colour
    {
        public float Red;
        public float Green;
        public float Blue;
        public float Alpha;
    }

    public class Render
    {
        private DirectX directX;

        private ComPtr<ID3D11RenderTargetView> renderTargetView = default;
        private ComPtr<ID3D11Texture2D> framebuffer;

        private ComPtr<ID3D11Buffer> vertexBuffer = default;
        private ComPtr<ID3D11Buffer> indexBuffer = default;
        private ComPtr<ID3D11Buffer> colourBuffer = default;
        private ComPtr<ID3D11Buffer> transformBuffer = default;

        private Camera camera = new Camera();

        public ComPtr<ID3D11RenderTargetView> RenderTargetView => renderTargetView;
        public Camera Camera { get; set; }

        public Render(DirectX directX)
        {
            this.directX = directX;
        }

        public void ClearScreen(Colour backgroundColour)
        {
            unsafe
            {
                framebuffer = directX.Swapchain.GetBuffer<ID3D11Texture2D>(0);

                SilkMarshal.ThrowHResult(directX.Device.CreateRenderTargetView(framebuffer, null, ref renderTargetView));

                float[] backgroundColour2 = { backgroundColour.Red, backgroundColour.Green, backgroundColour.Blue, backgroundColour.Alpha };
                fixed (float* colorPtr = backgroundColour2)
                {
                    directX.DeviceContext.ClearRenderTargetView(renderTargetView, colorPtr);
                }
            }
        }

        public void SetViewport(Vector2D<int> FramebufferSize)
        {
            var viewport = new Viewport(0, 0, FramebufferSize.X, FramebufferSize.Y, 0, 1);

            //Console.WriteLine($"Viewport size: {EngineWindow._window.FramebufferSize}");

            directX.DeviceContext.RSSetViewports(1, in viewport);
        }

        public void SetRenderTargetView()
        {
            directX.DeviceContext.OMSetRenderTargets(1, ref renderTargetView, ref Unsafe.NullRef<ID3D11DepthStencilView>());
        }

        public void SetDefaultRasterizerState()
        {
            unsafe
            {
                var rasterizerDesc = new RasterizerDesc
                {
                    FillMode = FillMode.Solid,
                    CullMode = CullMode.None, // Turn off culling
                    FrontCounterClockwise = false
                };

                ComPtr<ID3D11RasterizerState> rasterizerState = default;
                directX.Device.CreateRasterizerState(in rasterizerDesc, ref rasterizerState);
                directX.DeviceContext.RSSetState(rasterizerState);
            }
        }

        public void Assemble(Mesh mesh, uint vertexStride = 3U * sizeof(float), uint vertexOffset = 0U, D3DPrimitiveTopology topology = D3DPrimitiveTopology.D3D10PrimitiveTopologyTrianglelist)
        {
            vertexBuffer = directX.VertexBuffer;
            indexBuffer = directX.IndexBuffer;
            colourBuffer = directX.ColourBuffer;

            directX.DeviceContext.IASetPrimitiveTopology(topology);
            directX.DeviceContext.IASetInputLayout(mesh.InputLayout);

            // Bind position buffer to slot 0
            directX.DeviceContext.IASetVertexBuffers(0, 1, ref vertexBuffer, in vertexStride, in vertexOffset);

            // Bind color buffer to slot 1
            uint colorStride = 3U * sizeof(float); // 3 floats per color (RGB)
            uint colorOffset = 0U;
            directX.DeviceContext.IASetVertexBuffers(1, 1, ref colourBuffer, in colorStride, in colorOffset);

            directX.DeviceContext.IASetIndexBuffer(indexBuffer, Format.FormatR32Uint, 0);

            UpdateTransformBuffer(
                mesh.Transform.ModelMatrix,
                camera.ViewMatrix,
                camera.ProjectionMatrix
            );
        }
        public void BindShader()
        {
            ComPtr<ID3D11ClassInstance> nullClassInstance = default;
            directX.DeviceContext.VSSetShader(directX.VertexShader, ref nullClassInstance, 0);
            directX.DeviceContext.PSSetShader(directX.PixelShader, ref nullClassInstance, 0);
        }

        public void DrawQuad(int indicesLength)
        {
            directX.DeviceContext.DrawIndexed((uint)indicesLength, 0, 0);
        }

        public void Present()
        {
            directX.Swapchain.Present(1, 0);
        }

        public unsafe void CreateTransformBuffer()
        {
            var bufferDesc = new BufferDesc
            {
                ByteWidth = (uint)(sizeof(Matrix4X4<float>) * 3), // Model, View, Projection
                Usage = Usage.Dynamic,
                BindFlags = (uint)BindFlag.ConstantBuffer,
                CPUAccessFlags = (uint)CpuAccessFlag.Write
            };

            // Create the buffer
            transformBuffer = default;
            SilkMarshal.ThrowHResult(
                directX.Device.CreateBuffer(in bufferDesc, null, ref transformBuffer)
            );

            directX.TransformBuffer = transformBuffer;
        }

        public void UpdateTransformBuffer(Matrix4X4<float> model, Matrix4X4<float> view, Matrix4X4<float> projection)
        {
            unsafe
            {
                // Check if the transformBuffer is not initialized
                if (transformBuffer.Handle == null)
                {
                    CreateTransformBuffer();
                }

                try
                {
                    // Map the buffer to get access to its memory
                    var mappedResource = default(MappedSubresource);
                    directX.DeviceContext.Map(transformBuffer, 0, Map.WriteDiscard, 0, ref mappedResource);

                    // Get a pointer to the buffer data
                    var dataPtr = (float*)mappedResource.PData;
                    if (dataPtr == null)
                    {
                        throw new InvalidOperationException("Failed to map constant buffer memory");
                    }

                    // Write all matrices in a single block
                    Span<Matrix4X4<float>> matrices = stackalloc Matrix4X4<float>[3] { model, view, projection };
                    fixed (Matrix4X4<float>* matricesPtr = matrices)
                    {
                        // Copy all matrices at once (48 floats total)
                        System.Buffer.MemoryCopy(
                            matricesPtr,
                            dataPtr,
                            3 * sizeof(Matrix4X4<float>),
                            3 * sizeof(Matrix4X4<float>)
                        );
                    }
                }
                finally
                {
                    // Always unmap the buffer when done
                    directX.DeviceContext.Unmap(transformBuffer, 0);
                }

                // Bind the buffer to the vertex shader
                directX.DeviceContext.VSSetConstantBuffers(0, 1, ref transformBuffer);
            }
        }


        public void Cleanup()
        {
            renderTargetView.Dispose();
        }
    }
}
