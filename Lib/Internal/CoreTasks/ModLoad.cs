using CustomizeLib.BepInEx.Internal.Datas;
using CustomizeLib.BepInEx.Internal.Mod;
using CustomizeLib.BepInEx.Internal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.CoreTasks
{
    /// <summary>
    /// Lib初始化
    /// </summary>
    internal struct ModLoad : ILoadTask
    {
        // 优先做初始化
        readonly int IModTask.GetOrder() => IModTask.Low;

        readonly void ILoadTask.OnLoad()
        {
            // 初始化数据类
            LibData.Instance = new();
            RegData.Instance = new();

            // 初始化数据
            LibData.Instance.Logger = ModCore.Instance.Log;

            // 初始化控制台
            InternalTools.ConsoleTools_SetOutputEncoding();
        }
    }
}
