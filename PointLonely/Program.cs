using Qubit.Engine;
using Qubit.Engine.Core;

namespace PointLonely
{
    internal class Program
    {
        static void Main(string[] args)
        {
            QEngine engine = new(
                new WindowOptions(1280, 720, "Point Lonely, built with QubitEngine"),
                new AppLogic(),
                new Input()
            );
            engine.Run();
        }

    }
}
