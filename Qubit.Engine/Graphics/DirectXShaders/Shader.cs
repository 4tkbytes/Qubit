using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qubit.Engine.Core;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;

namespace Qubit.Engine.Graphics.DirectXShaders
{
    public class Shader
    {
        private Shader2 vertexShader;
        private Shader2 pixelShader;
        private Buffer buffer;

        private float[] vertices; 
        private uint[] indices; 
        private string vertexShaderCode; 
        private string pixelShaderCode;

        private ComPtr<ID3D11InputLayout> inputLayout;
        public ComPtr<ID3D11InputLayout> InputLayout => inputLayout;

        public Shader(float[] vertices, uint[] indices, string vertexShaderCode, string pixelShaderCode)
        {
            this.vertices = vertices;
            this.indices = indices;
            this.vertexShaderCode = vertexShaderCode;
            this.pixelShaderCode = pixelShaderCode;

            if (EngineWindow.directX == null)
                throw new InvalidOperationException("DirectX instance is not initialized");

            buffer = new(vertices, indices, EngineWindow.directX);

            vertexShader = new(vertexShaderCode, Shader2.ShaderType.Vertex);
            pixelShader = new(pixelShaderCode, Shader2.ShaderType.Pixel);

            unsafe
            {
                layoutDesc();
            }

            vertexShader.Cleanup();
            pixelShader.Cleanup();
        }

        private unsafe void layoutDesc()
        {
            if (EngineWindow.directX == null)
                throw new InvalidOperationException("DirectX instance is not initialized");
                
            fixed (byte* name = SilkMarshal.StringToMemory("POS"))
            {
                var inputElement = new InputElementDesc
                {
                    SemanticName = name,
                    SemanticIndex = 0,
                    Format = Format.FormatR32G32B32Float,
                    InputSlot = 0,
                    AlignedByteOffset = 0,
                    InputSlotClass = InputClassification.PerVertexData,
                    InstanceDataStepRate = 0
                };

                inputLayout = default;
                SilkMarshal.ThrowHResult
                (
                    EngineWindow.directX.Device.CreateInputLayout
                    (
                        in inputElement,
                        1,
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
