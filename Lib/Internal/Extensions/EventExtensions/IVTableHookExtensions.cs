using BepInEx.Unity.IL2CPP.Hook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.Extensions.EventExtensions
{
    internal static class IVTableHookExtensions
    {
        internal static TDelegate[] GetOriginals<TDelegate>(this IVTableHook<TDelegate> hook) where TDelegate : Delegate =>
            hook.Originals;

        public static uint GetCallingIdx<TDelegate>(this IVTableHook<TDelegate> hook) where TDelegate : Delegate =>
            hook.CallingIdx;

        public static object? CallCurrent<TDelegate>(this IVTableHook<TDelegate> hook, params object[] param) where TDelegate : Delegate =>
            hook.GetOriginals()[hook.GetCallingIdx()].DynamicInvoke(param);
    }
}
