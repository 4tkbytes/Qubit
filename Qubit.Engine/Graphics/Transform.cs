using Silk.NET.Maths;

namespace Qubit.Engine.Graphics
{
    public class Transform
    {
        public Vector3D<float> Position { get; set; } = Vector3D<float>.Zero;
        public Vector3D<float> Rotation { get; set; } = Vector3D<float>.Zero;
        public Vector3D<float> Scale { get; set; } = new Vector3D<float>(1, 1, 1);

        public Matrix4X4<float> ModelMatrix => CalculateModelMatrix();

        public Transform() { }

        private Matrix4X4<float> CalculateModelMatrix()
        {
            // Create rotation matrices for each axis
            var rotX = Matrix4X4.CreateRotationX(Rotation.X);
            var rotY = Matrix4X4.CreateRotationY(Rotation.Y);
            var rotZ = Matrix4X4.CreateRotationZ(Rotation.Z);

            // Combine rotations
            var rotation = rotX * rotY * rotZ;

            // Create scale and translation matrices
            var scale = Matrix4X4.CreateScale(Scale.X, Scale.Y, Scale.Z);
            var translation = Matrix4X4.CreateTranslation(Position.X, Position.Y, Position.Z);

            // Combine all transformations (scale, then rotate, then translate)
            return scale * rotation * translation;
        }

        public void Rotate(float deltaX, float deltaY, float deltaZ)
        {
            Rotation += new Vector3D<float>(deltaX, deltaY, deltaZ);
        }

        public void RotateDegrees(float deltaX, float deltaY, float deltaZ)
        {
            float toRadians = MathF.PI / 180.0f;
            Rotate(deltaX * toRadians, deltaY * toRadians, deltaZ * toRadians);
        }

        public void Translate(float deltaX, float deltaY, float deltaZ)
        {
            Position += new Vector3D<float>(deltaX, deltaY, deltaZ);
        }

        public void ScaleBy(float factorX, float factorY, float factorZ)
        {
            Scale *= new Vector3D<float>(factorX, factorY, factorZ);
        }

        public void ScaleUniform(float factor)
        {
            ScaleBy(factor, factor, factor);
        }
        public void Reset()
        {
            Position = Vector3D<float>.Zero;
            Rotation = Vector3D<float>.Zero;
            Scale = new Vector3D<float>(1, 1, 1);
        }

    }
}
