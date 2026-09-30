using CustomizeLib.BepInEx.Internal.CoreTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx
{
    public partial interface IInitHarmony
    {
        void IInitTask.OnInit()
        {
            EnableInitHarmony.AddPatch(GetOriginal(), GetPrefix()?.Method, GetPostfix()?.Method, GetTranspiler()?.Method, GetFinalizer()?.Method, GetILManipulator()?.Method);
        }
    }
}
