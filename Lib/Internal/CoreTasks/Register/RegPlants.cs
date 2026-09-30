using CustomizeLib.BepInEx.Internal.Datas;
using CustomizeLib.BepInEx.Internal.Extensions;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.CoreTasks.Register
{
    /// <summary>
    /// 注册二创植物
    /// </summary>
    internal struct RegPlants : IGameAppEvent
    {
        public RegPlants() { }

        internal Dictionary<ID, CustomPlant> CustomPlants { get; set; } = [];

        readonly IGameAppEvent.GameAppEvent IGameAppEvent.Filter => IGameAppEvent.GameAppEvent.PreLoadResources;

        readonly void IGameAppEvent.OnEvent()
        {
            foreach (var type in IModData.GetAll(typeof(IModPlant<>)))
            {
                var data = IModData.GetData<CustomPlant>(type, "CustomPlantData");

                if (!ClassInjector.IsTypeRegisteredInIl2Cpp(type))
                    ClassInjector.RegisterTypeInIl2Cpp(type);

                data.Prefab.AddComponent(type.GetGenericArguments()[0].Il2CppType());
                data.Prefab.AddComponent(type.Il2CppType());

                if (GameAPP.resourcesManager.allPlants.Contains(data.Id)) continue; // 如果已有则跳过

                GameAPP.resourcesManager.plantPrefabs[data.Id] = data.Prefab;
                GameAPP.resourcesManager.plantPrefabs[data.Id].tag = "Plant"; // 打tag
                GameAPP.resourcesManager.plantPreviews[data.Id] = data.Preview;
                GameAPP.resourcesManager.plantPreviews[data.Id].tag = "Preview"; // 打tag
                GameAPP.resourcesManager.allPlants.Add(data.Id);
                PlantDataManager.PlantData_Default.Add(data.Id, data.ToPlantData()); // 添加plantdata

                foreach (var (a, b) in data.Fusions)
                    MixData.AddOrderedRecipe(a, b, data.Id);
            }
        }
    }
}
