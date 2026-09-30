using CustomizeLib.BepInEx.Internal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.CoreTasks
{
    internal struct SetDetourEnvReady : IGameAppEvent
    {
        readonly IGameAppEvent.GameAppEvent IGameAppEvent.Filter => IGameAppEvent.GameAppEvent.PostAwake;

        readonly void IGameAppEvent.OnEvent() => IDetourHook.CachedEnvReady = true;
    }
}
