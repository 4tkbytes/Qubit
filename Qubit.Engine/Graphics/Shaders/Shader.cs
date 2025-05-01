using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Qubit.Engine.Core;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;

namespace Qubit.Engine.Graphics.DirectXShaders
{
    public class Shader
    {
        string shaderSource;
        ShaderType shaderType;

        // Private compiled stuff
        private ComPtr<ID3D11VertexShader> vertexShader;
        private ComPtr<ID3D11PixelShader> pixelShader;

        private ComPtr<ID3D10Blob> vertexCode = default;
        private ComPtr<ID3D10Blob> vertexErrors = default;

        private ComPtr<ID3D10Blob> pixelCode = default;
        private ComPtr<ID3D10Blob> pixelErrors = default;

        // Public stuff
        public ComPtr<ID3D11VertexShader> VertexShader => vertexShader;
        public ComPtr<ID3D11PixelShader> PixelShader => pixelShader;
        public ComPtr<ID3D10Blob> VertexCode => vertexCode;


        public enum ShaderType
        {
            Vertex,
            Pixel,
        }

        public Shader(string shaderSource, ShaderType type)
        {
            this.shaderSource = shaderSource;
            shaderType = type;

            var shaderBytes = Encoding.ASCII.GetBytes(shaderSource);
            switch (type)
            {
                case ShaderType.Vertex:
                    unsafe { LoadVertex(shaderBytes); }
                    return;
                case ShaderType.Pixel:
                    unsafe { LoadPixel(shaderBytes); }
                    return;
            }
        }

        public unsafe void LoadVertex(byte[] resourceContents)
        {
            HResult hr = EngineWindow.directX.Compiler.Compile
            (
                in resourceContents[0],               // Pass the first byte by reference
                (nuint)resourceContents.Length,       // Length of the shader source
                (string)null,                         // Optional: source file name (null if not needed)
                null,                                 // Optional: defines (null if not needed)
                ref Unsafe.NullRef<ID3DInclude>(),    // Optional: include handler (null if not needed)
                "vs_main",                            // Entry point name
                "vs_5_0",                             // Target shader model
                0,                                    // Flags1
                0,                                    // Flags2
                ref vertexCode,                       // Compiled shader code
                ref vertexErrors                      // Compilation errors
            );

            if (hr.IsFailure)
            {
                if (vertexErrors.Handle is not null)
                {
                    Console.WriteLine(SilkMarshal.PtrToString((nint)vertexErrors.GetBufferPointer()));
                }

                hr.Throw();
            }

            vertexShader = default;

            SilkMarshal.ThrowHResult
            (
                EngineWindow.directX.Device.CreateVertexShader
                (
                    vertexCode.GetBufferPointer(),
                    vertexCode.GetBufferSize(),
                    ref Unsafe.NullRef<ID3D11ClassLinkage>(),
                    ref vertexShader
                )
            );

            EngineWindow.directX.VertexShader = vertexShader;
        }

        public unsafe void LoadPixel(byte[] resourceContents)
        {
            HResult hr = EngineWindow.directX.Compiler.Compile
            (
                in resourceContents[0],               // Pass the first byte by reference
                (nuint)resourceContents.Length,       // Length of the shader source
                (string)null,                         // Optional: source file name (null if not needed)
                null,                                 // Optional: defines (null if not needed)
                ref Unsafe.NullRef<ID3DInclude>(),    // Optional: include handler (null if not needed)
                "ps_main",                            // Entry point name
                "ps_5_0",                             // Target shader model
                0,                                    // Flags1
                0,                                    // Flags2
                ref pixelCode,                        // Compiled shader code
                ref pixelErrors                       // Compilation errors
            );

            if (hr.IsFailure)
            {
                if (pixelErrors.Handle is not null)
                {
                    Console.WriteLine(SilkMarshal.PtrToString((nint)pixelErrors.GetBufferPointer()));
                }

                hr.Throw();
            }

            pixelShader = default;

            SilkMarshal.ThrowHResult
            (
                EngineWindow.directX.Device.CreatePixelShader
                (
                    pixelCode.GetBufferPointer(),
                    pixelCode.GetBufferSize(),
                    ref Unsafe.NullRef<ID3D11ClassLinkage>(),
                    ref pixelShader
                )
            );

            EngineWindow.directX.PixelShader = pixelShader;
        }

        public void Cleanup()
        {
            switch (shaderType)
            {
                case ShaderType.Vertex:
                    vertexCode.Dispose();
                    vertexErrors.Dispose();
                    return;
                case ShaderType.Pixel:
                    pixelCode.Dispose();
                    pixelErrors.Dispose();
                    return;
            }
            return;
        }
    }
}
