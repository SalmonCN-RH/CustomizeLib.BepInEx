using AlmanacData;
using CustomizeLib.BepInEx.Internal.Datas;
using CustomizeLib.BepInEx.Internal.Mod;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.Tasks
{
    /// <summary>
    /// Lib初始化
    /// </summary>
    internal struct ModLoad : ILoadTask
    {
        // 优先做初始化
        public readonly int GetOrder() => CuLibPriority.Low;

        public readonly void OnLoad()
        {
            // 初始化数据类
            LibData.Instance = new();
            RegData.Instance = new();

            // 初始化数据
            LibData.Instance.Logger = ModCore.Instance.Log;

            // 初始化控制台
            Utils.ConsoleUtils.SetOutputEncoding();

            ModRegister.Register();
        }
    }

    internal static class ModRegister
    {
        internal static void Register()
        {
            RegisterPlants();
        }

        /// <summary>
        /// 注册植物
        /// </summary>
        private static void RegisterPlants()
        {
            IGameAppEvent.AddCallback(new(GameAppEvent.PostStart, () =>
            {
                foreach (var type in IModData.GetAll(typeof(IModPlant<>)))
                {
                    try
                    {
                        // 植物注册
                        var data = IModData.GetData<CustomPlant>(type, nameof(IModPlant<>.CustomPlant));

                        if (!ClassInjector.IsTypeRegisteredInIl2Cpp(type))
                            ClassInjector.RegisterTypeInIl2Cpp(type);

                        var basePlant = type.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IModPlant<>));
                        if (basePlant == null)
                        {
                            Logger.LogWarning($"Type {type} cannot find the TBase closed generic parameter of IModPlant<TBase>");
                            continue;
                        }
                        data.Prefab.AddComponent(basePlant.GetGenericArguments()[0].Il2CppType());
                        data.Prefab.AddComponent(type.Il2CppType());

                        if (GameAPP.resourcesManager.allPlants.Contains(data.Id)) continue; // 如果已有则跳过

                        GameAPP.resourcesManager.plantPrefabs[data.Id] = data.Prefab;
                        GameAPP.resourcesManager.plantPrefabs[data.Id].tag = "Plant"; // 打tag
                        GameAPP.resourcesManager.plantPreviews[data.Id] = data.Preview;
                        GameAPP.resourcesManager.plantPreviews[data.Id].tag = "Preview"; // 打tag
                        GameAPP.resourcesManager.allPlants.Add(data.Id);
                        PlantDataManager.PlantData_Default.Add(data.Id, data.ToPlantData()); // 添加plantdata
                        PlantDataManager.PlantData_Modified.Add(data.Id, data.ToPlantData()); // 添加plantdata

                        foreach (var (a, b) in data.Fusions)
                            MixData.AddOrderedRecipe(a, b, data.Id);

                        // 图鉴注册
                        var almanac = IModData.GetData<CustomPlantAlmanac?>(type, nameof(IModPlant<>.CustomPlantAlmanac));
                        if (almanac != null)
                        {
                            Utils.NativeClassUtils.Initialize(typeof(AlmanacDataLoader));
                            AlmanacDataLoader.plantDatas[data.Id] = almanac.Value.ToPlantInfo();
                            PlantDataManager.unlocked[data.Id] = PlantDataManager.CheckIfPlantUnlock(data.Id);
                        }
                    }
                    catch (Exception e)
                    {
                        Utils.ExceptionUtils.FormatAndLogException(e);
                    }
                }

                PlantDataManager.ApplyModify();
            }));
        }
    }
}
