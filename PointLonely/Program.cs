using Qubit.Engine;
using Qubit.Engine.Core;
using Qubit.Engine.Graphics;
using Qubit.Engine.Input;
using Qubit.Engine.Scene;
using Silk.NET.Core.Native;
using Silk.NET.Input;
using Silk.NET.Maths;

namespace PointLonely
{
    public class Program : IAppLogic, IInput
    {
        float[] vertices =
        {
            //  X      Y      Z
             0.5f,  0.5f,  0.0f,  // top right
             0.5f, -0.5f,  0.0f,  // bottom right
            -0.5f, -0.5f,  0.0f,  // bottom left
            -0.5f,  0.5f,  0.0f,  // top left
        };


        uint[] indices =
        {
            0, 1, 3,
            1, 2, 3,
        };


        // Rainbow colors for the vertices
        // Fixed vertex colors - Vibrant pure colors
        float[] colors = {
            1.0f, 0.0f, 0.0f, // Red (top right)
            0.0f, 1.0f, 0.0f, // Green (bottom right)
            0.0f, 0.0f, 1.0f, // Blue (bottom left)
            1.0f, 1.0f, 0.0f  // Yellow (top left)
        };



        private Scene scene;

        public static QEngine Engine { get; private set; }

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
            scene = new("default");

            var mesh = new Mesh(
                vertices,
                indices,
                colors,
                Qubit.Engine.Utils.File.GetEmbeddedResourceString("Qubit.Engine.Resources.default_vertex.hlsl"),
                Qubit.Engine.Utils.File.GetEmbeddedResourceString("Qubit.Engine.Resources.default_pixel.hlsl")
            );

            // Position the mesh where the camera can see it (at Z=0)
            mesh.Transform.Position = new Vector3D<float>(0, 0, 2.0f);

            // Setup the camera properly
            scene.Camera.Position = new Vector3D<float>(0, 0, 0);  // Camera at origin
            scene.Camera.Target = new Vector3D<float>(0, 0, 1);    // Looking along +Z axis
            scene.Camera.Up = Vector3D<float>.UnitY;               // Y is up

            // Set the aspect ratio based on window dimensions
            scene.Camera.AspectRatio = 1280f / 720f;

            scene.AddMesh("quad", mesh);
        }

        void IAppLogic.OnRender(double deltaTime)
        {
            Render render = new Render(EngineWindow.directX);

            render.ClearScreen(new Colour{ Red = 0.0f, Blue = 0.0f, Green = 0.0f, Alpha = 1.0f});

            scene.Render(render);

            render.Present();

            render.Cleanup();
        }

        void IAppLogic.OnUpdate(double deltaTime)
        {
            var mesh = scene.MeshList["quad"];

            // More dramatic rotation to see color changes clearly
            mesh.Transform.Rotate(0, (float)deltaTime * 1.0f, 0); // Rotate around Y axis
        }

        public void KeyDown(IKeyboard keyboard, Key key, int keycode)
        {

        }

    }
}
