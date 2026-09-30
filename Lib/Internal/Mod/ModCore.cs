using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx.Internal.Tools;
using System.Reflection;

namespace CustomizeLib.BepInEx.Internal.Mod
{
    [BepInPlugin(ModInfo.Core_GUID, ModInfo.Core_NAME, ModInfo.Core_VER)]
    [BepInPriority(BepInPriority.High + 1)]
    internal class ModCore : BasePlugin
    {
        internal static ModCore Instance { get; private set; } = null!;

        public override void Load()
        {
            Instance = this;

            var dlls = InternalTools.TypeTools_GetAllAssemblies();

            var methods = InternalTools.TypeTools_GetAllMethods(
                types: [.. InternalTools.TypeTools_GetAllDerivedTypes<IModTask>(InternalTools.TypeTools_GetAllAssemblies()).
                                Where(t => t != typeof(IInitTask) && !t.IsGenericType)],
                filter: (t) => InternalTools.TypeTools_TryGetMethodWithFlag(t, "RunAll", InternalTools.TypeTools_DefaultDeclaredOnly));
            foreach (var method in methods)
            {
                try
                {
                    method.Invoke(null, []);
                }
                catch (Exception ex)
                {
                    Logger.LogError(
                        $"Error in execute IModTask: \n" +
                        $"Message: {ex.Message}\n" +
                        $"{ex.StackTrace}\n" +
                        $"ToString: \n" +
                        $"{ex}");
                }
            }
        }
    }
}
