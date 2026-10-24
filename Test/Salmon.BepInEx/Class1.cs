using CustomizeLib.BepInEx;
using UnityEngine;

[assembly: CuLibLoad]

namespace Salmon.BepInEx
{
    public class Salmon : MonoBehaviour, IModPlant<Plant>, ILoadTask
    {
        public CustomizeLib.BepInEx.CustomPlant CustomPlant => new()
        {
            Sun = 127,
            AttackDamage = 0,
            AttackInterval = 0f,
            CD = 0f,
            Fusions = [],
            Id = 1905,
            MaxHealth = 2147483647,
            Prefab = Utils.AssetBundleUtils.GetAssetBundle("salmon").GetAsset<GameObject>("SalmonPrefab"),
            Preview = Utils.AssetBundleUtils.GetAssetBundle("salmon").GetAsset<GameObject>("SalmonPreview")
        };

        public void OnLoad()
        {
            Console.WriteLine("a");
        }
    }
}
