using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Qubit.Engine.Core;
using Qubit.Engine.Graphics.DirectXShaders;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;

namespace Qubit.Engine.Graphics
{
    public class Mesh
    {
        private Shader vertexShader;
        private Shader pixelShader;
        private DirectXShaders.Buffer buffer;

        private float[] vertices;
        private uint[] indices;
        private float[] colours;

        private string vertexShaderCode; 
        private string pixelShaderCode;

        private ComPtr<ID3D11InputLayout> inputLayout;
        public ComPtr<ID3D11InputLayout> InputLayout => inputLayout;
        public float[] Vertices => vertices;
        public uint[] Indices => indices;
        public float[] Colors => colours;
        public int VertexCount => vertices?.Length / 3 ?? 0;
        public int IndicesCount => indices?.Length ?? 0;

        public Mesh(float[] vertices, uint[] indices, float[] colours, string vertexShaderCode, string pixelShaderCode)
        {
            this.vertices = vertices;
            this.indices = indices;
            this.colours = colours;

            this.vertexShaderCode = vertexShaderCode;
            this.pixelShaderCode = pixelShaderCode;

            if (EngineWindow.directX == null)
                throw new InvalidOperationException("DirectX instance is not initialized");

            buffer = new(vertices, indices, colours, EngineWindow.directX);

            vertexShader = new(vertexShaderCode, Shader.ShaderType.Vertex);
            pixelShader = new(pixelShaderCode, Shader.ShaderType.Pixel);

            unsafe
            {
                layoutDesc();
            }

            vertexShader.Cleanup();
            pixelShader.Cleanup();
        }

        public Mesh(float[] vertices, uint[] indices, float[] colours)
            : this(vertices, indices, colours,
                Utils.File.GetEmbeddedResourceString("Qubit.Engine.Resources.default_vertex.hlsl"),
                Utils.File.GetEmbeddedResourceString("Qubit.Engine.Resources.default_pixel.hlsl"))
        {
        }

        private unsafe void layoutDesc()
        {
            if (EngineWindow.directX == null)
                throw new InvalidOperationException("DirectX instance is not initialized");

            // Create an array to hold both input elements
            var inputElements = stackalloc InputElementDesc[2];

            // In layoutDesc(), print the buffer information
            Console.WriteLine($"Vertex code buffer size: {vertexShader.VertexCode.GetBufferSize()}");
            Console.WriteLine("Creating input layout with POSITION and COLOR semantics");

            // Position element
            fixed (byte* positionName = SilkMarshal.StringToMemory("POSITION"))
            fixed (byte* colorName = SilkMarshal.StringToMemory("COLOR"))
            {
                // Position input element
                inputElements[0] = new InputElementDesc
                {
                    SemanticName = positionName,
                    SemanticIndex = 0,
                    Format = Format.FormatR32G32B32Float,
                    InputSlot = 0,
                    AlignedByteOffset = 0,
                    InputSlotClass = InputClassification.PerVertexData,
                    InstanceDataStepRate = 0
                };

                // Color input element
                inputElements[1] = new InputElementDesc
                {
                    SemanticName = colorName,
                    SemanticIndex = 0,
                    Format = Format.FormatR32G32B32Float,
                    InputSlot = 1, // Use slot 1 for color buffer
                    AlignedByteOffset = 0,
                    InputSlotClass = InputClassification.PerVertexData,
                    InstanceDataStepRate = 0
                };

                inputLayout = default;
                SilkMarshal.ThrowHResult
                (
                    EngineWindow.directX.Device.CreateInputLayout
                    (
                        inputElements,
                        2, // Two input elements
                        vertexShader.VertexCode.GetBufferPointer(),
                        vertexShader.VertexCode.GetBufferSize(),
                        ref inputLayout
                    )
                );
                EngineWindow.directX.InputLayout = inputLayout;
            }
        }

        



    }
}
