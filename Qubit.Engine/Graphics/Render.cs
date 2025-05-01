using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Qubit.Engine.Core;
using Qubit.Engine.Graphics.DirectXShaders;
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

        private ComPtr<ID3D11Buffer> vertexBuffer;
        private ComPtr<ID3D11Buffer> indexBuffer;
        private ComPtr<ID3D11Buffer> colourBuffer;


        public ComPtr<ID3D11RenderTargetView> RenderTargetView => renderTargetView;

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

        public void Assemble(DirectXShaders.Mesh mesh, uint vertexStride = 3U * sizeof(float), uint vertexOffset = 0U, D3DPrimitiveTopology topology = D3DPrimitiveTopology.D3D10PrimitiveTopologyTrianglelist)
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

        public void Cleanup()
        {
            renderTargetView.Dispose();
        }
    }
}
