using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Silk.NET.Maths;

namespace Qubit.Engine.Graphics
{
    public class Camera
    {
        public Vector3D<float> Position { get; set; } = new Vector3D<float>(0, 0, -3);
        public Vector3D<float> Target { get; set; } = Vector3D<float>.Zero;
        public Vector3D<float> Up { get; set; } = Vector3D<float>.UnitY;

        public float FieldOfView { get; set; } = 45.0f;
        public float AspectRatio { get; set; } = 16.0f / 9.0f;
        public float NearPlane { get; set; } = 0.1f;
        public float FarPlane { get; set; } = 100.0f;

        public Matrix4X4<float> ViewMatrix => CalculateViewMatrix();
        public Matrix4X4<float> ProjectionMatrix => CalculateProjectionMatrix();

        private Matrix4X4<float> CalculateViewMatrix()
        {
            return Matrix4X4.CreateLookAt(Position, Target, Up);
        }

        private Matrix4X4<float> CalculateProjectionMatrix()
        {
            return Matrix4X4.CreatePerspectiveFieldOfView(
                FieldOfView * (MathF.PI / 180.0f), // Convert degrees to radians
                AspectRatio,
                NearPlane,
                FarPlane
            );
        }

    }
}
