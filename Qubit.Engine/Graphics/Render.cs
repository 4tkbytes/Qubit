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
                // Always recreate the transform buffer
                CreateTransformBuffer();

                try
                {
                    // Map the buffer
                    var mappedResource = default(MappedSubresource);
                    directX.DeviceContext.Map(transformBuffer, 0, Map.WriteDiscard, 0, ref mappedResource);

                    if (mappedResource.PData == null)
                        throw new InvalidOperationException("Failed to map constant buffer memory");

                    // Copy the matrices to a continuous memory block
                    float* dataPtr = (float*)mappedResource.PData;

                    // Debug: Log projection matrix values
                    Console.WriteLine("Projection Matrix:");
                    for (int row = 0; row < 4; row++)
                    {
                        string rowStr = "";
                        for (int col = 0; col < 4; col++)
                        {
                            rowStr += $"{projection[row, col]:F3} ";
                        }
                        Console.WriteLine(rowStr);
                    }

                    // Transpose the matrices if needed for DirectX (row-major vs column-major)
                    Matrix4X4<float> transposedModel = Matrix4X4.Transpose(model);
                    Matrix4X4<float> transposedView = Matrix4X4.Transpose(view);
                    Matrix4X4<float> transposedProj = Matrix4X4.Transpose(projection);

                    // Copy the transposed matrices (ensures correct format for HLSL)
                    CopyMatrixToBuffer(transposedModel, dataPtr);
                    CopyMatrixToBuffer(transposedView, dataPtr + 16);
                    CopyMatrixToBuffer(transposedProj, dataPtr + 32);
                }
                finally
                {
                    // Always unmap
                    directX.DeviceContext.Unmap(transformBuffer, 0);
                }

                // Bind the buffer
                directX.DeviceContext.VSSetConstantBuffers(0, 1, ref transformBuffer);
            }
        }

        private unsafe void CopyMatrixToBuffer(Matrix4X4<float> matrix, float* destination)
        {
            destination[0] = matrix.M11;
            destination[1] = matrix.M12;
            destination[2] = matrix.M13;
            destination[3] = matrix.M14;
            destination[4] = matrix.M21;
            destination[5] = matrix.M22;
            destination[6] = matrix.M23;
            destination[7] = matrix.M24;
            destination[8] = matrix.M31;
            destination[9] = matrix.M32;
            destination[10] = matrix.M33;
            destination[11] = matrix.M34;
            destination[12] = matrix.M41;
            destination[13] = matrix.M42;
            destination[14] = matrix.M43;
            destination[15] = matrix.M44;
        }



        public void Cleanup()
        {
            renderTargetView.Dispose();
        }
    }
}
