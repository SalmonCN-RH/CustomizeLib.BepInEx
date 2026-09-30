using CustomizeLib.BepInEx.Internal.Extensions;
using CustomizeLib.BepInEx.Internal.Extensions.EventExtensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.Patches.Harmony.PlantEvent
{
    [HarmonyPatch(typeof(SavePlantData))]
    internal static class SavePlantData_RunEvent
    {
        [HarmonyPatch(nameof(SavePlantData.LoadData))]
        [HarmonyPrefix]
        public static void PreLoadData(SavePlantData __instance, Plant __0)
        {
            if (!__0.TryGetBindEvents(out var list)) return;

            // 处理逻辑
            list.DoAll(e => IPlantEvent.EventSingleMaps[IPlantEvent.EventType.AfterDeserialized].Invoke(e, Trigger.Pre, [__instance]));
        }

        [HarmonyPatch(nameof(SavePlantData.LoadData))]
        [HarmonyPostfix]
        public static void PostLoadData(SavePlantData __instance, Plant __0)
        {
            if (!__0.TryGetBindEvents(out var list)) return;

            // 处理逻辑
            list.DoAll(e => IPlantEvent.EventSingleMaps[IPlantEvent.EventType.AfterDeserialized].Invoke(e, Trigger.Post, [__instance]));
        }
    }
}
