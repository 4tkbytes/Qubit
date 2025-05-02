using Qubit.Engine;
using Qubit.Engine.Core;
using Qubit.Engine.Graphics;
using Qubit.Engine.Input;
using Qubit.Engine.Scene;
using Silk.NET.Input;
using Silk.NET.Maths;

namespace PointLonely
{
    public class Program : IAppLogic, IInput
    {
        float[] vertices =
        {
            // X     Y     Z
            -0.9f,  0.9f, 0.0f,  // top left
             0.9f,  0.9f, 0.0f,  // top right
             0.9f, -0.9f, 0.0f,  // bottom right
            -0.9f, -0.9f, 0.0f,  // bottom left
        };

        uint[] indices =
        {
            0, 1, 3,
            1, 2, 3,
        };

        float[] colors = {
            1.0f, 0.0f, 0.0f, // Red (top left - vertex 0)
            0.0f, 1.0f, 0.0f, // Green (top right - vertex 1)
            0.0f, 0.0f, 1.0f, // Blue (bottom right - vertex 2)
            1.0f, 1.0f, 0.0f  // Yellow (bottom left - vertex 3)
        };

        private Scene scene;
        private Render render;

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

            // Position the mesh directly in front of the camera
            mesh.Transform.Position = new Vector3D<float>(0, 0, 2.0f);

            // No rotation
            mesh.Transform.Rotation = Vector3D<float>.Zero;

            // Make it larger to ensure it's visible
            mesh.Transform.Scale = new Vector3D<float>(2, 2, 2);

            // Set up a simple orthographic camera
            scene.Camera.Position = new Vector3D<float>(0, 0, 0);
            scene.Camera.Target = new Vector3D<float>(0, 0, 1);
            scene.Camera.Up = Vector3D<float>.UnitY;

            // Use orthographic projection for simplicity
            scene.Camera.FieldOfView = 45.0f;
            scene.Camera.AspectRatio = 1280f / 720f;
            scene.Camera.NearPlane = 0.1f;
            scene.Camera.FarPlane = 100.0f;
            scene.Camera.UseOrthographic = true;
            scene.Camera.OrthographicSize = 1.0f; // Adjust as needed

            scene.AddMesh("quad", mesh);
            render = new Render(EngineWindow.directX);
        }


        void IAppLogic.OnRender(double deltaTime)
        {
            // Set a dark gray background
            render.ClearScreen(new Colour { Red = 0.2f, Blue = 0.2f, Green = 0.2f, Alpha = 1.0f });

            scene.Render(render);

            render.Present();
            render.Cleanup();
        }



        void IAppLogic.OnUpdate(double deltaTime)
        {
            var mesh = scene.MeshList["quad"];

            mesh.Transform.Rotate(0, (float)deltaTime, 0);
        }



        public void KeyDown(IKeyboard keyboard, Key key, int keycode)
        {

        }

    }
}
