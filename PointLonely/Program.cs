using System.Reflection;
using System.Runtime.CompilerServices;
using Qubit.Engine;
using Qubit.Engine.Core;
using Qubit.Engine.Graphics;
using Qubit.Engine.Graphics.DirectXShaders;
using Qubit.Engine.Input;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D.Compilers;
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


        // Rainbow colors for the vertices
        float[] colors = {
            1.0f, 0.0f, 0.0f, // red (bottom left)
            0.0f, 1.0f, 0.0f, // green (bottom right)
            0.0f, 0.0f, 1.0f, // blue (top right)
            1.0f, 1.0f, 0.0f  // yellow (top left)
        };

        public static QEngine Engine { get; private set; }
        private Mesh mesh;

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
            mesh = new Mesh(
                vertices,
                indices,
                colors,
                Qubit.Engine.Utils.File.GetEmbeddedResourceString("Qubit.Engine.Resources.default_vertex.hlsl"),
                Qubit.Engine.Utils.File.GetEmbeddedResourceString("Qubit.Engine.Resources.default_pixel.hlsl")
            );
        }

        void IAppLogic.OnRender(double deltaTime)
        {
            if (EngineWindow.directX == null)
            {
                throw new Exception("DirectX has not been initialised");
            }
            Render render = new Render(EngineWindow.directX);

            render.ClearScreen(new Colour{ Red = 0.0f, Blue = 0.0f, Green = 0.0f, Alpha = 1.0f});

            render.SetViewport(EngineWindow._window.FramebufferSize);
            render.SetRenderTargetView();
            render.SetDefaultRasterizerState();

            render.Assemble(mesh, 3U * sizeof(float), 0U, D3DPrimitiveTopology.D3D11PrimitiveTopologyTrianglelist);
            render.BindShader();

            render.DrawQuad(indices.Length);
            render.Present();

            render.Cleanup();
        }

        void IAppLogic.OnUpdate(double deltaTime)
        {

        }

        public void KeyDown(IKeyboard keyboard, Key key, int keycode)
        {

        }

    }
}
