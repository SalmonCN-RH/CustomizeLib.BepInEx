using BepInEx.Unity.IL2CPP.Hook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.Extensions.EventExtensions
{
    internal static class IApplyDetourExtensions
    {
        internal static TDelegate[] GetOriginals<TDelegate>(this IDetourHook<TDelegate> detour) where TDelegate : Delegate =>
            [.. detour.Originals];

        public static INativeDetour[] GetDetours<TDelegate>(this IDetourHook<TDelegate> detour) where TDelegate : Delegate =>
            [.. detour.Detours];

        public static int GetCallOriginalIdx<TDelegate>(this IDetourHook<TDelegate> detour) where TDelegate : Delegate =>
            detour.CallOriginalIdx;
    }
}
