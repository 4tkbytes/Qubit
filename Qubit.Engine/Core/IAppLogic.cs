using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Silk.NET.Core.Native;
using Silk.NET.Maths;
using Silk.NET.OpenXR;

namespace Qubit.Engine.Core
{
    public interface IAppLogic
    {
        void OnLoad();

        void OnUpdate(double deltaTime);

        void OnRender(double deltaTime);
    }
}
