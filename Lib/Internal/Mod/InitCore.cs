using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx.Internal.CoreTasks;
using CustomizeLib.BepInEx.Internal.Datas;
using CustomizeLib.BepInEx.Internal.Tools;
using HarmonyLib;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.Mod
{
    [BepInPriority(BepInPriority.Low - 1)]
    [BepInPlugin(ModInfo.PreInit_GUID, ModInfo.PreInit_NAME, ModInfo.PreInit_VER)]
    internal class InitCore : BasePlugin
    {
        public override void Load()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (dir != null)
            {
                // 遍历所有 .dll 和 .exe
                foreach (var file in Directory.EnumerateFiles(dir, "*.dll", SearchOption.AllDirectories))
                    TryLoadDll(file);
                foreach (var file in Directory.EnumerateFiles(dir, "*.exe", SearchOption.AllDirectories))
                    TryLoadDll(file);
            }

            LibData.Instance.Logger = Log;

            var methods = InternalTools.TypeTools_GetAllMethods(
                types: InternalTools.TypeTools_GetAllDerivedTypes<IInitTask>(InternalTools.TypeTools_GetAllAssemblies()),
                filter: (t) => InternalTools.TypeTools_TryGetMethodWithFlag(t, "RunAll", InternalTools.TypeTools_DefaultDeclaredOnly));
            foreach (var method in methods)
            {
                try
                {
                    method.Invoke(null, []);
                }
                catch (Exception ex)
                {
                    Log.LogError(
                        $"Error in execute {nameof(IInitTask)}: \n" +
                        $"Message: {ex.Message}\n" +
                        $"{ex.StackTrace}\n" +
                        $"ToString: \n" +
                        $"{ex}");
                }
            }
        }

        private static void TryLoadDll(string path)
        {
            try
            {
                var asmName = AssemblyName.GetAssemblyName(path);

                foreach (var asm in InternalTools.TypeTools_GetAllAssemblies())
                    if (AssemblyName.ReferenceMatchesDefinition(asm.GetName(), asmName))
                        return;

                using var assembly = AssemblyDefinition.ReadAssembly(path, new()
                {
                    ReadSymbols = false,
                    ReadingMode = ReadingMode.Deferred,
                    InMemory = true
                });

                Logger.LogInfo($"{assembly.FullName}");
                if (assembly.CustomAttributes.Any(attr => attr.AttributeType.FullName == typeof(CustomizeLibLoadAttribute).FullName))
                    Assembly.LoadFrom(path);
            }
            catch (BadImageFormatException) { }
            catch (FileLoadException) { }
            catch (Exception ex)
            {
                Logger.LogError(
                    $"Load failed: {path}: \n" +
                    $"Message: {ex.Message}\n" +
                    $"{ex.StackTrace}\n" +
                    $"ToString: \n" +
                    $"{ex}");
            }
        }
    }
}
