using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DotNet.PlatformAbstractions;
using Qubit.Engine.Graphics;
using Qubit.Engine.Input;
using Silk.NET.Core.Native;
using Silk.NET.DXGI;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using static Qubit.Engine.Core.EngineWindow;

namespace Qubit.Engine.Core
{
    public class EngineWindow
    {
        public static IWindow? _window;
        private readonly IAppLogic appLogic;
        private readonly IInput inputLogic;
        private IInputContext? inputContext;
        public static DirectX? directX;

        public enum GraphicsAPI
        {
            OpenGL,
            Vulkan,
            DirectX,
            Any
        }

        public EngineWindow(WindowOptions classOptions, IAppLogic appLogic, IInput inputLogic, GraphicsAPI graphicsAPI)
        {
            this.appLogic = appLogic;
            this.inputLogic = inputLogic;

            var api = GetGraphicsAPI(graphicsAPI);

            if (!api.Equals(Silk.NET.Windowing.GraphicsAPI.None))
            {
                throw new Exception("Only DirectX is supported, unable to run");
            }

            Silk.NET.Windowing.WindowOptions options = Silk.NET.Windowing.WindowOptions.Default with
            {
                Size = new Vector2D<int>(classOptions.width, classOptions.height),
                Title = classOptions.title,
                API = api
            };

            _window = Silk.NET.Windowing.Window.Create(options);

            SubscribeToEvents();
            _window.Load += OnLoad;
        }

        private void OnLoad()
        {
            if (_window == null)
                throw new InvalidOperationException("Window not initialized");

            // Initialise DirectX
            unsafe
            {
                directX = new();
            }

            // Make this last
            inputContext = _window.CreateInput();

            // Set up keyboard handling
            for (int i = 0; i < inputContext.Keyboards.Count; i++)
            {
                inputContext.Keyboards[i].KeyDown += OnKeyDown;
            }

            // Call application's OnLoad AFTER DirectX is initialized
            appLogic.OnLoad();
        }

        internal void Cleanup()
        {
            directX?.Cleanup();
            _window?.Dispose();
        }

        private void OnKeyDown(IKeyboard keyboard, Key key, int keycode)
        {
            // Forward the key event to the application's input handler
            inputLogic.KeyDown(keyboard, key, keycode);

            if (key == Key.Escape)
            {
                _window?.Close();
            }
        }

        public void Run()
        {
            if (_window == null)
                throw new InvalidOperationException("Window not initialized");
                
            _window.Run();
        }

        internal void SubscribeToEvents()
        {
            if (_window == null)
                throw new InvalidOperationException("Window not initialized");

            // Don't subscribe to Load here - we'll call it manually after DirectX init
            // _window.Load += appLogic.OnLoad;
            _window.Update += appLogic.OnUpdate;
            _window.Render += appLogic.OnRender;
            _window.FramebufferResize += OnFramebufferResize;
        }

        void OnFramebufferResize(Vector2D<int> newSize)
        {
            if (directX == null)
                throw new InvalidOperationException("DirectX not initialized");
                
            SilkMarshal.ThrowHResult
            (
                directX.Swapchain.ResizeBuffers(0, (uint)newSize.X, (uint)newSize.Y, Format.FormatB8G8R8A8Unorm, 0)
            );
        }

        private static Silk.NET.Windowing.GraphicsAPI GetGraphicsAPI(GraphicsAPI graphicsAPI)
        {
            switch (graphicsAPI)
            {
                case GraphicsAPI.DirectX:
                    // DirectX
                    return Silk.NET.Windowing.GraphicsAPI.None;

                case GraphicsAPI.OpenGL:
                    return Silk.NET.Windowing.GraphicsAPI.Default;

                case GraphicsAPI.Vulkan:
                    return Silk.NET.Windowing.GraphicsAPI.DefaultVulkan;

                case GraphicsAPI.Any:
                    var platform = Environment.OSVersion.Platform;
                    switch (platform)
                    {
                        case PlatformID.Win32NT:
                            // DirectX
                            return Silk.NET.Windowing.GraphicsAPI.None;
                        case PlatformID.Unix:
                            return Silk.NET.Windowing.GraphicsAPI.Default;
                        case PlatformID.MacOSX:
                            return Silk.NET.Windowing.GraphicsAPI.Default;
                        default:
                            Console.WriteLine("Unknown operating system.");
                            break;
                    }
                    break;
            }

            throw new Exception("Unable to determine OS");
        }
    }

    public class WindowOptions
    {
        public int width;
        public int height;
        public String title;

        public WindowOptions(
            int width,
            int height,
            String title
            )
        {
            this.width = width;
            this.height = height;
            this.title = title;
        }
    }
}
