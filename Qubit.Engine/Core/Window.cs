using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Qubit.Engine.Input;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace Qubit.Engine.Core
{
    internal class Window
    {
        public static IWindow _window;
        IAppLogic appLogic;
        IInput inputLogic;
        IInputContext inputContext;

        public Window(WindowOptions classOptions, IAppLogic appLogic, IInput inputLogic)
        {
            this.appLogic = appLogic;
            this.inputLogic = inputLogic;

            Silk.NET.Windowing.WindowOptions options = Silk.NET.Windowing.WindowOptions.Default with
            {
                Size = new Vector2D<int>(classOptions.width, classOptions.height),
                Title = classOptions.title
            };

            _window = Silk.NET.Windowing.Window.Create(options);

            SubscribeToEvents();
            _window.Load += OnLoad;
        }

        private void OnLoad()
        {
            inputContext = _window.CreateInput();

            // Set up keyboard handling
            for (int i = 0; i < inputContext.Keyboards.Count; i++)
            {
                inputContext.Keyboards[i].KeyDown += OnKeyDown;
            }
        }

        private void OnKeyDown(IKeyboard keyboard, Key key, int keycode)
        {
            // Forward the key event to the application's input handler
            inputLogic.KeyDown(keyboard, key, keycode);

            if (key == Key.Escape)
            {
                _window.Close();
            }
        }

        public void Run()
        {
            _window.Run();
        }

        internal void SubscribeToEvents()
        {
            _window.Load += appLogic.OnLoad;
            _window.Update += appLogic.OnUpdate;
            _window.Render += appLogic.OnRender;
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
