using Silk.NET.Maths;

namespace Qubit.Engine.Graphics
{
    public class Transform
    {
        public Vector3D<float> Position { get; set; } = Vector3D<float>.Zero;
        public Vector3D<float> Rotation { get; set; } = Vector3D<float>.Zero;
        public Vector3D<float> Scale { get; set; } = new Vector3D<float>(1, 1, 1);

        public Matrix4X4<float> ModelMatrix => CalculateModelMatrix();

        // This fuckass function is the reason why I sleep so late
        private Matrix4X4<float> CalculateModelMatrix()
        {
            // Use Silk.NET.Math libraries for all matrix operations

            // Create scale matrix
            var scale = Matrix4X4.CreateScale(Scale.X, Scale.Y, Scale.Z);

            // Create rotation matrices for each axis in the correct order
            var rotX = Matrix4X4.CreateRotationX(Rotation.X);
            var rotY = Matrix4X4.CreateRotationY(Rotation.Y);
            var rotZ = Matrix4X4.CreateRotationZ(Rotation.Z);

            // Combine rotations carefully (order matters)
            // For most 3D engines, rotation order is typically Z, then X, then Y
            var rotation = Matrix4X4.Multiply(Matrix4X4.Multiply(rotZ, rotX), rotY);

            // Create translation matrix
            var translation = Matrix4X4.CreateTranslation(Position.X, Position.Y, Position.Z);

            // Combine transforms in the correct order: Scale → Rotate → Translate
            // Use explicit Matrix4X4.Multiply to avoid any operator overloading issues
            var scaleRotate = Matrix4X4.Multiply(scale, rotation);
            var finalTransform = Matrix4X4.Multiply(scaleRotate, translation);

            return finalTransform;
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
