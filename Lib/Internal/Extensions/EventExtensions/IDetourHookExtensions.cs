using BepInEx.Unity.IL2CPP.Hook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.Extensions.EventExtensions
{
    internal static class IDetourHookExtensions
    {
        internal static TDelegate GetOriginal<TDelegate>(this IDetourHook<TDelegate> detour) where TDelegate : Delegate =>
            detour.Original;

        public static INativeDetour GetDetour<TDelegate>(this IDetourHook<TDelegate> detour) where TDelegate : Delegate =>
            detour.Detour;
    }
}
