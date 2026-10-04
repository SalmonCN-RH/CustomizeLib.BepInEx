using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

namespace SuperFurnameMod.BepInEx
{
    [BepInPlugin("salmon.superfurnacemod", "SuperFurnaceMod", "1.0")]
    public sealed class SuperFurnaceMod : BasePlugin
    {
        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            // ClassInjector.RegisterTypeInIl2Cpp<MyBrainBehaviour>();
        }
    }

    //public class MyBrainBehaviour : MonoBehaviour
    //{
    //    public void OnTriggerEnter2D(Collider2D collision)
    //    {
    //        if (collision != null && collision.TryGetComponent<Zombie>(out var zombie) && zombie != null)
    //        {
    //            var brain = GetComponent<Brain>();
    //            if (zombie.theZombieRow == brain.theRow)
    //            {
    //                brain.theHealth = 1;
    //            }
    //        }
    //    }
    //}

    //[HarmonyPatch(typeof(Brain))]
    //public static class BrainPatch
    //{
    //    [HarmonyPatch(nameof(Brain.Awake))]
    //    [HarmonyPostfix]
    //    public static void PostAwake(Brain __instance)
    //    {
    //        __instance.AddComponent<MyBrainBehaviour>();
    //    }
    //}

    [HarmonyPatch(typeof(SuperFurnace))]
    public static class SuperFurnacePatch
    {
        [HarmonyPatch(nameof(SuperFurnace.Near))]
        [HarmonyPrefix]
        public static bool PreNear()
        {
            return false;
        }
    }
}
