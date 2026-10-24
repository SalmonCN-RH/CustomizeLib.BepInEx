using BepInEx.Unity.IL2CPP.Hook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx
{
    public static partial class Extensions
    {
        /// <summary>
        /// 获取 <paramref name="detour"/> 的 Original
        /// </summary>
        /// <typeparam name="TDelegate">委托类型</typeparam>
        /// <param name="detour"><see cref="IDetourHook{TDelegate}"/> 实例</param>
        /// <returns>实例的 Original</returns>
        public static TDelegate GetOriginal<TDelegate>(this IDetourHook<TDelegate> detour) where TDelegate : Delegate =>
            detour.Original;

        /// <summary>
        /// 获取 <paramref name="detour"/> 的 Detour
        /// </summary>
        /// <typeparam name="TDelegate">委托类型</typeparam>
        /// <param name="detour"><see cref="IDetourHook{TDelegate}"/> 实例</param>
        /// <returns>实例的 Detour</returns>
        public static INativeDetour GetDetour<TDelegate>(this IDetourHook<TDelegate> detour) where TDelegate : Delegate =>
            detour.Detour;
    }
}
