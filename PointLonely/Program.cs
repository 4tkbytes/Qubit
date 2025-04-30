using System.Reflection;
using System.Runtime.CompilerServices;
using Qubit.Engine;
using Qubit.Engine.Core;
using Qubit.Engine.Graphics.DirectXShaders;
using Qubit.Engine.Input;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;
using Silk.NET.Input;

namespace PointLonely
{
    public class Program : IAppLogic, IInput
    {
        float[] vertices =
        {
            //  X      Y      Z
             0.5f,  0.5f,  0.0f,
             0.5f, -0.5f,  0.0f,
            -0.5f, -0.5f,  0.0f,
            -0.5f,  0.5f,  0.5f,
        };

        uint[] indices =
        {
            0, 1, 3,
            1, 2, 3,
        };
        public static QEngine Engine { get; private set; }
        private Shader shader;

        static void Main(string[] args)
        {
            Engine = new(
                new WindowOptions(1280, 720, "Point Lonely, built with QubitEngine"),
                new Program(), new Program()
            );
            Engine.Run();
        }

        void IAppLogic.OnLoad()
        {
            shader = new Shader(
                vertices,
                indices,
                Qubit.Engine.Utils.File.GetEmbeddedResourceString("Qubit.Engine.Resources.default_vertex.hlsl"),
                Qubit.Engine.Utils.File.GetEmbeddedResourceString("Qubit.Engine.Resources.default_pixel.hlsl")
            );
        }

        void IAppLogic.OnRender(double deltaTime)
        {
            if (EngineWindow.directX == null || shader == null)
                return;

            // Access the DirectX context
            var directX = EngineWindow.directX;
            var deviceContext = directX.DeviceContext;
            var swapchain = directX.Swapchain;
            var device = directX.Device;

            // Create background color (black)
            float[] backgroundColor = new float[] { 0.0f, 0.0f, 0.0f, 1.0f };

            unsafe
            {
                // Obtain the framebuffer for the swapchain's backbuffer
                using var framebuffer = swapchain.GetBuffer<ID3D11Texture2D>(0);

                // Create a view over the render target
                ComPtr<ID3D11RenderTargetView> renderTargetView = default;
                SilkMarshal.ThrowHResult(device.CreateRenderTargetView(framebuffer, null, ref renderTargetView));

                // Clear the render target to be black ahead of rendering
                fixed (float* colorPtr = backgroundColor)
                {
                    deviceContext.ClearRenderTargetView(renderTargetView, colorPtr);
                }

                // Fixing CS0176 by qualifying '_window' with the type name 'EngineWindow'
                var viewport = new Viewport(0, 0, EngineWindow._window.FramebufferSize.X, EngineWindow._window.FramebufferSize.Y, 0, 1);
                deviceContext.RSSetViewports(1, in viewport);

                // Tell the output merger about our render target view
                deviceContext.OMSetRenderTargets(1, ref renderTargetView, ref Unsafe.NullRef<ID3D11DepthStencilView>());

                // Update the input assembler to use triangle list topology
                deviceContext.IASetPrimitiveTopology(D3DPrimitiveTopology.D3DPrimitiveTopologyTrianglelist);

                // Access your shader resources
                // This assumes your Buffer class exposes VertexBuffer and IndexBuffer properties
                // You may need to adapt this based on your actual implementation
                var vertexBuffer = directX.VertexBuffer;
                var indexBuffer = directX.IndexBuffer;
                var vertexStride = (uint)sizeof(float) * 3; // 3 floats per vertex (X, Y, Z)
                var vertexOffset = 0u;

                // Update the input assembler with vertex and index buffers
                deviceContext.IASetInputLayout(shader.InputLayout);
                deviceContext.IASetVertexBuffers(0, 1, ref vertexBuffer, in vertexStride, in vertexOffset);
                deviceContext.IASetIndexBuffer(indexBuffer, Format.FormatR32Uint, 0);

                // Bind the shaders - this assumes your Shader2 class exposes VertexShader and PixelShader properties
                var vsShader = directX.VertexShader;
                var psShader = directX.PixelShader;

                ComPtr<ID3D11ClassInstance> nullClassInstance = default;
                deviceContext.VSSetShader(vsShader, ref nullClassInstance, 0);
                deviceContext.PSSetShader(psShader, ref nullClassInstance, 0);

                // Draw the quad (6 indices for 2 triangles)
                deviceContext.DrawIndexed((uint)indices.Length, 0, 0);

                // Present the drawn image
                swapchain.Present(1, 0);

                // Clean up
                renderTargetView.Dispose();
            }
        }

        void IAppLogic.OnUpdate(double deltaTime)
        {

        }

        public void KeyDown(IKeyboard keyboard, Key key, int keycode)
        {
        }

    }
}
