using CustomizeLib.BepInEx;
using UnityEngine;

[assembly: CustomizeLibLoad]
namespace LibTest.BepInEx
{
    public sealed class MyMod : ILoadTask
    {
        public void OnLoad()
        {
            Debug.Log($"my mod run");
        }
    }
}
