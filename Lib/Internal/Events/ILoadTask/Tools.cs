using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx
{
    public partial interface ILoadTask
    {
        /// <summary>
        /// 执行所有 <see cref="ILoadTask"/>
        /// </summary>
        internal static void RunAll() => DoAll<ILoadTask>();
    }
}
