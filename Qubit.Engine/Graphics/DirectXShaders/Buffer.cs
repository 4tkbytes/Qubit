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

        public Buffer(float[] vertices, uint[] indices)
        {
            this.vertices = vertices;
            this.indices = indices;

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
                    Graphics.DirectX.Device.CreateBuffer(
                        in bufferDesc,
                        in subresourceData,
                        ref Graphics.DirectX.VertexBuffer
                ));
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
                    Graphics.DirectX.Device.CreateBuffer(
                        in bufferDesc,
                        in subresourceData, 
                        ref Graphics.DirectX.IndexBuffer
                ));
            }

        }

    }

}
