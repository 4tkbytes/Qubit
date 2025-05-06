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
            // VO
            -0.5f, 0.5f, 0.5f,
            // V1
            -0.5f, -0.5f, 0.5f,
            // V2
            0.5f, -0.5f, 0.5f,
            // V3
            0.5f, 0.5f, 0.5f,
            // V4
            -0.5f, 0.5f, -0.5f,
            // V5
            0.5f, 0.5f, -0.5f,
            // V6
            -0.5f, -0.5f, -0.5f,
            // V7
            0.5f, -0.5f, -0.5f,
        };

        uint[] indices =
        {
            // Front face
            0, 1, 3, 3, 1, 2,
            // Top Face
            4, 0, 3, 5, 4, 3,
            // Right face
            3, 2, 7, 5, 3, 7,
            // Left face
            6, 1, 0, 6, 0, 4,
            // Bottom face
            2, 1, 6, 2, 6, 7,
            // Back face
            7, 6, 4, 7, 4, 5,
        };

        float[] colors = {
            0.5f, 0.0f, 0.0f,
            0.0f, 0.5f, 0.0f,
            0.0f, 0.0f, 0.5f,
            0.0f, 0.5f, 0.5f,
            0.5f, 0.0f, 0.0f,
            0.0f, 0.5f, 0.0f,
            0.0f, 0.0f, 0.5f,
            0.0f, 0.5f, 0.5f,
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
