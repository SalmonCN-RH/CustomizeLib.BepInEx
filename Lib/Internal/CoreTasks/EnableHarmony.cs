using BepInEx.Unity.IL2CPP.Hook;
using CustomizeLib.BepInEx.Internal.Datas;
using CustomizeLib.BepInEx.Internal.Extensions;
using CustomizeLib.BepInEx.Internal.Mod;
using CustomizeLib.BepInEx.Internal.Patches;
using CustomizeLib.BepInEx.Internal.Test;
using CustomizeLib.BepInEx.Internal.Tools;
using HarmonyLib;
using HarmonyLib.Public.Patching;
using HarmonyLib.Tools;
using Il2CppInterop.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.CoreTasks
{
    /// <summary>
    /// 启用HarmonyPatch (Mod加载前)
    /// </summary>
    internal struct EnableInitHarmony : IInitTask
    {
        readonly int IModTask.GetOrder() => IModTask.High;

        internal static List<InitHarmony> InitHarmonies { get; set; } = [];
        internal static List<InitHarmony> LoadedHarmonies { get; set; } = [];

        internal static void AddPatch(MethodBase original, MethodInfo? prefix = null, MethodInfo? postfix = null, MethodInfo? transpiler = null, MethodInfo? finalizer = null,
            MethodInfo? ilmanipulator = null) => InitHarmonies.Add(InitHarmony.GetNew(original, prefix, postfix, transpiler, finalizer, ilmanipulator));

        readonly void IInitTask.OnInit()
        {
            foreach (var item in InitHarmonies)
            {
                EnablePatch(item);
                LoadedHarmonies.Add(item);
            }
        }

        private static PatchProcessor EnablePatch(InitHarmony harmony)
        {
            var patch = EnableHarmony.HarmonyInstance.CreateProcessor(harmony.Original);
            patch.AddPrefix(harmony.Prefix);
            patch.AddPostfix(harmony.Postfix);
            patch.AddTranspiler(harmony.Transpiler);
            patch.AddFinalizer(harmony.Finalizer);
            patch.AddILManipulator(harmony.ILManipulator);
            patch.Patch();
            return patch;
        }

        internal struct InitHarmony
            (MethodBase original, MethodInfo? prefix = null, MethodInfo? postfix = null, MethodInfo? transpiler = null, MethodInfo? finalizer = null, 
            MethodInfo? ilmanipulator = null)
        {
            [NotNull] internal readonly MethodBase Original => original;
            internal readonly HarmonyMethod? Prefix => prefix != null ? new(prefix) : null;
            internal readonly HarmonyMethod? Postfix => postfix != null ? new(postfix) : null;
            internal readonly HarmonyMethod? Transpiler => transpiler != null ? new(transpiler) : null;
            internal readonly HarmonyMethod? Finalizer => finalizer != null ? new(finalizer) : null;
            internal readonly HarmonyMethod? ILManipulator => ilmanipulator != null ? new(ilmanipulator) : null;

            internal static InitHarmony GetNew(MethodBase original, MethodInfo? prefix = null, MethodInfo? postfix = null, MethodInfo? transpiler = null,
                MethodInfo? finalizer = null, MethodInfo? ilmanipulator = null)
            {
                return new(original, prefix, postfix, transpiler, finalizer, ilmanipulator);
            }

            internal readonly InitHarmony SetPrefix(MethodInfo prefix) =>
                new(original, prefix, postfix, transpiler, finalizer, ilmanipulator);
            internal readonly InitHarmony SetPostfix(MethodInfo postfix) =>
                new(original, prefix, postfix, transpiler, finalizer, ilmanipulator);
            internal readonly InitHarmony SetTranspiler(MethodInfo transpiler) =>
                new(original, prefix, postfix, transpiler, finalizer, ilmanipulator);
            internal readonly InitHarmony SetFinalizer(MethodInfo finalizer) =>
                new(original, prefix, postfix, transpiler, finalizer, ilmanipulator);

            internal readonly InitHarmony SetIlManipulator(MethodInfo ilmanipulator) =>
                new(original, prefix, postfix, transpiler, finalizer, ilmanipulator);
        }
    }

    /// <summary>
    /// 启用HarmonyPatch
    /// </summary>
    internal struct EnableHarmony : ILoadTask
    {
        internal static Harmony HarmonyInstance = new(ModInfo.Core_GUID);

        readonly void ILoadTask.OnLoad()
        {
            HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
