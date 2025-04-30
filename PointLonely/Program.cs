using System.Reflection;
using Qubit.Engine;
using Qubit.Engine.Core;
using Qubit.Engine.Graphics.DirectXShaders;
using Qubit.Engine.Input;
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

        }

        void IAppLogic.OnUpdate(double deltaTime)
        {

        }

        public void KeyDown(IKeyboard keyboard, Key key, int keycode)
        {
        }

    }
}
