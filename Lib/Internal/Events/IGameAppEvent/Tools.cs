using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx
{
    public partial interface IGameAppEvent
    {
        internal static void ManualRunAll(GameAppEvent call)
        {
            foreach (var task in GetAllTask<IGameAppEvent>())
                if ((task.Filter & call) != 0) task.Do();

            RunAllCallback<IGameAppEvent>([call]);
        }

        public static void AddCallback(GameAppEventCallback callback) => AddCallback<IGameAppEvent>(callback);
    }

    [Flags]
    public enum GameAppEvent
    {
        Default = 0,
        PreAwake = 1,
        PostAwake = 2,
        PreStart = 4,
        PostStart = 8,
        PreLoadResources = 16,
        PostLoadResources = 32,
        All = PreAwake | PostAwake | PreStart | PostStart | PreLoadResources | PostLoadResources
    }

    public struct GameAppEventCallback(GameAppEvent filter, Action callback) : ITaskCallback
    {
        public readonly void Execute(object?[] args)
        {
            if ((filter & (GameAppEvent)args[0]!) != 0)
                callback.Invoke();
        }
    }
}
