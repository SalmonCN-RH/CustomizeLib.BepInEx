using CustomizeLib.BepInEx.Internal.Auxiliary;
using CustomizeLib.BepInEx.Internal.Extensions;
using CustomizeLib.BepInEx.Internal.Extensions.EventExtensions;
using CustomizeLib.BepInEx.Internal.Tools;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.Patches.Detour.PlantEvent
{
    internal delegate void SavePlantData_Cons(IntPtr @this, IntPtr plant, IntPtr method);

    //internal struct SavePlantData_RunEvent : IDetourHook<SavePlantData_Cons>
    //{
    //    public SavePlantData_Cons GetHookFunction() => Hook;
    //    public IEnumerable<IntPtr> GetHookTargets()
    //    {
    //        Logger.LogInfo($"ret {InternalTools.DetourTools_GetConstructor(typeof(SavePlantData), typeof(Plant)).MethodPointer:X}");
    //        yield return InternalTools.DetourTools_GetConstructor(typeof(SavePlantData), typeof(Plant)).MethodPointer;
    //    }

    //    internal void Hook(IntPtr @this, IntPtr plant, IntPtr method)
    //    {
    //        var p = new Plant(plant);
    //        Logger.LogInfo($"run on {p.thePlantType} {p.thePlantRow} {p.thePlantColumn} {this.GetCallOriginalIdx()}");
    //        var data = new SavePlantData(@this);

    //        if (p.TryGetBindEvents(out var list))
    //            list.DoAll((i) => IPlantEvent.EventSingleMaps[IPlantEvent.EventType.BeforeSerialized].Invoke(i, Trigger.Pre, [data]));

    //        this.GetOriginals()[this.GetCallOriginalIdx()].Invoke(@this, plant, method);

    //        if (list.Any())
    //            list.DoAll((i) => IPlantEvent.EventSingleMaps[IPlantEvent.EventType.BeforeSerialized].Invoke(i, Trigger.Post, [data]));
    //    }

    //    public void OnApplyHook()
    //    {
    //        _ = ClearZombie();
    //    }

    //    internal async Task ClearZombie()
    //    {
    //        while (true)
    //        {
    //            await UniTask.Delay(3000, cancellationToken: Il2CppSystem.Threading.CancellationToken.None);
    //            if (GameAPP.Instance != null && GameAPP.Instance.TryGetComponent<CheatKey>(out var key))
    //            {
    //                if (key.CheatKeys.ContainsKey("clearzombie"))
    //                    key.CheatKeys["clearzombie"].Invoke();
    //            }
    //        }
    //    }
    //}
}
