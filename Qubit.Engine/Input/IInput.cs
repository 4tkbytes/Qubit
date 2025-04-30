using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Silk.NET.Input;

namespace Qubit.Engine.Input
{
    public interface IInput
    {
        void KeyDown(IKeyboard keyboard, Key key, int keycode);
    }
}
