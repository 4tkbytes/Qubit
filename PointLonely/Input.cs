using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qubit.Engine.Input;
using Silk.NET.Input;

namespace PointLonely
{
    internal class Input : IInput
    {
        public void KeyDown(IKeyboard keyboard, Key key, int keycode)
        {
            // Handle keyboard input here
            Console.WriteLine($"Key pressed: {key}");

            // Add your application-specific keyboard handling logic
        }
    }

}
