using Qubit.Engine.Core;
using Qubit.Engine.Input;

// Note: DO NOT USE ANY SILK.NET REFERENCES IN THIS FILE, KEEP IT CLEAN

namespace Qubit.Engine
{
    public class QEngine
    {
        WindowOptions options;
        private EngineWindow window;
        public EngineWindow Window => window;

        public QEngine(WindowOptions options, IAppLogic appLogic, IInput inputLogic)
        {
            this.options = options;

            window = new(options, appLogic, inputLogic, EngineWindow.GraphicsAPI.Any);
        }

        public void Run()
        {
            window.Run();
            window.Cleanup();
        }
    }

}