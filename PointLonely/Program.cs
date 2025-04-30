using System.Reflection;
using Qubit.Engine;
using Qubit.Engine.Core;

namespace PointLonely
{
    public class Program
    {
        public static QEngine Engine { get; private set; }

        static void Main(string[] args)
        {
            Engine = new(
                new WindowOptions(1280, 720, "Point Lonely, built with QubitEngine"),
                new AppLogic(),
                new Input()
            );
            Engine.Run();
        }

    }
}
