using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;

namespace Qubit.Engine.Graphics.DirectXShaders
{
    internal class Buffer
    {
        float[] vertices;
        uint[] indices;
        private Graphics.DirectX directX; // Add a reference to the DirectX instance
        private ComPtr<ID3D11Buffer> vertexBuffer = default;
        private ComPtr<ID3D11Buffer> indexBuffer = default;

        public Buffer(float[] vertices, uint[] indices, Graphics.DirectX directX)
        {
            this.vertices = vertices;
            this.indices = indices;
            this.directX = directX; // Initialize the DirectX instance

            Load(vertices, indices);
        }

        private unsafe void Load(float[] vertices, uint[] indices)
        {
            var bufferDesc = new BufferDesc
            {
                ByteWidth = (uint)(vertices.Length * sizeof(float)),
                Usage = Usage.Default,
                BindFlags = (uint)BindFlag.VertexBuffer
            };

            fixed (float* vertexData = vertices)
            {
                var subresourceData = new SubresourceData
                {
                    PSysMem = vertexData
                };

                SilkMarshal.ThrowHResult(
                    directX.Device.CreateBuffer(
                        in bufferDesc,
                        in subresourceData,
                        ref vertexBuffer
                ));
                
                // Assign to DirectX after creation
                directX.VertexBuffer = vertexBuffer;
            }

            bufferDesc = new BufferDesc
            {
                ByteWidth = (uint)(indices.Length * sizeof(uint)),
                Usage = Usage.Default,
                BindFlags = (uint)BindFlag.IndexBuffer
            };

            fixed (uint* indexData = indices)
            {
                var subresourceData = new SubresourceData
                {
                    PSysMem = indexData
                };

                SilkMarshal.ThrowHResult(
                    directX.Device.CreateBuffer(
                        in bufferDesc,
                        in subresourceData,
                        ref indexBuffer
                ));
                
                // Assign to DirectX after creation
                directX.IndexBuffer = indexBuffer;
            }
        }
    }
}
