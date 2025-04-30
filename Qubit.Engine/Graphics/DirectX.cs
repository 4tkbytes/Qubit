using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Qubit.Engine.Core;
using Silk.NET.Core.Contexts;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D.Compilers;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;

namespace Qubit.Engine.Graphics
{
    public class DirectX
    {
        DXGI dxgi = null!;
        D3D11 d3d11 = null!;
        D3DCompiler compiler = null!;

        ComPtr<IDXGIFactory2> factory = default;
        ComPtr<IDXGISwapChain1> swapchain = default;
        ComPtr<ID3D11Device> device = default;
        ComPtr<ID3D11DeviceContext> deviceContext = default;
        ComPtr<ID3D11Buffer> vertexBuffer = default;
        ComPtr<ID3D11Buffer> indexBuffer = default;
        ComPtr<ID3D11VertexShader> vertexShader = default;
        ComPtr<ID3D11PixelShader> pixelShader = default;
        ComPtr<ID3D11InputLayout> inputLayout = default;

        // Getters
        public DXGI Dxgi => dxgi;
        public D3D11 D3d11 => d3d11;
        public D3DCompiler Compiler => compiler;

        public ComPtr<IDXGIFactory2> Factory => factory;
        public ComPtr<IDXGISwapChain1> Swapchain => swapchain;
        public ComPtr<ID3D11Device> Device => device;
        public ComPtr<ID3D11DeviceContext> DeviceContext => deviceContext;
        public ComPtr<ID3D11Buffer> VertexBuffer => vertexBuffer;
        public ComPtr<ID3D11Buffer> IndexBuffer => indexBuffer;
        public ComPtr<ID3D11VertexShader> VertexShader
        {
            get => vertexShader;
            set => vertexShader = value;
        }
        public ComPtr<ID3D11PixelShader> PixelShader
        {
            get => pixelShader;
            set => pixelShader = value;
        }
        public ComPtr<ID3D11InputLayout> InputLayout
        {
            get => inputLayout;
            set => inputLayout = value;
        }

        unsafe public DirectX()
        {
            // Whether or not to force use of DXVK on platforms where native DirectX implementations are available
            // This is assuming that only Windows can run this, but can be changed accordingly
            const bool forceDxvk = false;

            INativeWindowSource nativeWindow = EngineWindow._window;
            dxgi = DXGI.GetApi(nativeWindow, forceDxvk);
            d3d11 = D3D11.GetApi(nativeWindow, forceDxvk);
            compiler = D3DCompiler.GetApi();

            SilkMarshal.ThrowHResult
            (
                d3d11.CreateDevice
                (
                    default(ComPtr<IDXGIAdapter>),
                    D3DDriverType.Hardware,
                    Software: default,
                    (uint)CreateDeviceFlag.Debug,
                    null,
                    0,
                    D3D11.SdkVersion,
                    ref device,
                    null,
                    ref deviceContext
                )
            );
#if DEBUG
            if (OperatingSystem.IsWindows())
            {
                // Log debug messages for this device (given that we've enabled the debug flag). Don't do this in release code!
                device.SetInfoQueueCallback(msg => Console.WriteLine(SilkMarshal.PtrToString((nint)msg.PDescription)));
            }
#endif

            // Swapchain
            var swapChainDesc = new SwapChainDesc1
            {
                BufferCount = 2,
                Format = Format.FormatB8G8R8A8Unorm,
                BufferUsage = DXGI.UsageRenderTargetOutput,
                SwapEffect = SwapEffect.FlipDiscard,
                SampleDesc = new SampleDesc(1, 0)
            };

            // Create DXGI factory to allow us to create a swapchain
            factory = dxgi.CreateDXGIFactory<IDXGIFactory2>();

            SilkMarshal.ThrowHResult(
                factory.CreateSwapChainForHwnd
                (
                    device,
                    nativeWindow.Native!.DXHandle!.Value,
                    in swapChainDesc,
                    null,
                    ref Unsafe.NullRef<IDXGIOutput>(),
                    ref swapchain
                    )
                );
        }

        public void Cleanup()
        {
            factory.Dispose();
            swapchain.Dispose();
            device.Dispose();
            deviceContext.Dispose();
            vertexBuffer.Dispose();
            indexBuffer.Dispose();
            vertexShader.Dispose();
            pixelShader.Dispose();
            inputLayout.Dispose();
            compiler.Dispose();
            d3d11.Dispose();
            dxgi.Dispose();
        }
    }
}
