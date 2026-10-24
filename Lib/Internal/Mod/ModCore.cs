using BepInEx;
using BepInEx.Unity.IL2CPP;
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

            var dlls = Utils.TypeUtils.GetAllAssemblies();

            var methods = Utils.TypeUtils.GetAllMethods(
                types: [.. Utils.TypeUtils.GetAllDerivedTypes<ILoadTask>(Utils.TypeUtils.GetAllAssemblies()).Where(t => !t.IsGenericType), 
                        .. Utils.TypeUtils.GetAllDerivedTypes<IGenericTask>(Utils.TypeUtils.GetAllAssemblies()).Where(t => !t.IsGenericType)],
                filter: (t) => Utils.TypeUtils.TryGetMethodWithFlag(t, "RunAll", Utils.TypeUtils.DefaultDeclaredOnly));
            foreach (var method in methods)
            {
                try
                {
                    method.Invoke(null, []);
                }
                catch (Exception ex)
                {
                    Logger.LogError(
                        $"Error in execute ILoadTask or IGenericTask: \n" +
                        $"Message: {ex.Message}\n" +
                        $"{ex.StackTrace}\n" +
                        $"ToString: \n" +
                        $"{ex}");
                }
            }

            IModTask.RunAllCallback<ILoadTask>([]);
        }
    }
}
