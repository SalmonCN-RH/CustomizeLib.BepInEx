using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.Tasks
{
    internal struct ModInit : IInitTask
    {
        // 优先做初始化
        public readonly int GetOrder() => CuLibPriority.Low;

        public readonly void OnInit()
        {
            IInitTask.AddCallback(new(EnableInitHarmony.OnInit));
            IInitTask.AddCallback(new(() => _ = new Utils.LoggerUtils.LoggerWrapper()));
            IInitTask.AddCallback(new(RegisterTypeInIl2Cpp.ProcessAll));
        }
    }
}
