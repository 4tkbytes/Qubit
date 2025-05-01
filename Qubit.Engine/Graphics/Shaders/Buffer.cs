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
        float[] colours;
        private DirectX directX;
        private ComPtr<ID3D11Buffer> vertexBuffer = default;
        private ComPtr<ID3D11Buffer> indexBuffer = default;
        private ComPtr<ID3D11Buffer> colourBuffer = default;

        public Buffer(float[] vertices, uint[] indices, float[] colours, DirectX directX)
        {
            this.vertices = vertices;
            this.indices = indices;
            this.colours = colours;
            this.directX = directX;

            Load(vertices, indices, colours);
        }

        private unsafe void Load(float[] vertices, uint[] indices, float[] colours)
        {
            // Vertex Buffer
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

            // Index Buffer
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

            // Colour Buffer
            bufferDesc = new BufferDesc
            {
                ByteWidth = (uint)(colours.Length * sizeof(float)),
                Usage = Usage.Default,
                BindFlags = (uint)BindFlag.VertexBuffer
            };

            fixed (float* colorData = colours)
            {
                var subresourceData = new SubresourceData
                {
                    PSysMem = colorData
                };

                SilkMarshal.ThrowHResult(
                    directX.Device.CreateBuffer(
                        in bufferDesc,
                        in subresourceData,
                        ref colourBuffer
                ));

                // Assign the color buffer to DirectX - FIX HERE
                directX.ColourBuffer = colourBuffer;  // CORRECTED LINE
            }
        }


    }
}
