using BepInEx.Unity.IL2CPP.Hook;
using CustomizeLib.BepInEx.Internal;
using CustomizeLib.BepInEx.Internal.Tools;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static UnityEngine.UI.Image;

namespace CustomizeLib.BepInEx
{
    #region Mod事件
    public partial interface IModTask
    {
        /// <summary>
        /// 缓存数据
        /// </summary>
        public object CachedData
        {
            get => GetCachedData();
            set => SetCachedData(value);
        }

        /// <summary>
        /// 加载顺序, 值小的优先
        /// </summary>
        public int GetOrder() => Default;

        /// <summary>
        /// 当触发时
        /// </summary>
        public void Do();

        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <returns>数据</returns>
        public object GetCachedData() => CachedDatas[GetType()];
        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <typeparam name="T">转换类型</typeparam>
        /// <returns>数据</returns>
        public T GetCachedData<T>() => (T)CachedDatas[GetType()];
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="data">数据</param>
        public void SetCachedData(object data) => CachedDatas[GetType()] = data; 
    }

    /// <summary>
    /// 作为泛型 Task 的基类
    /// </summary>
    public partial interface IGenericTask : IModTask { }

    public partial interface ILoadTask : IModTask 
    {
        public void OnLoad();
        void IModTask.Do() => OnLoad();
    }

    public partial interface IGameAppEvent : IModTask
    {
        public GameAppEvent Filter => GameAppEvent.Default;

        public void OnEvent();
        void IModTask.Do() => OnEvent();
    }

    public partial interface IInitTask : IModTask 
    {
        public void OnInit();
        void IModTask.Do() => OnInit();
    }

    public partial interface IDetourHook<TDelegate> : IGenericTask where TDelegate : Delegate
    {
        public new IDetourHookData CachedData
        {
            get => GetCachedData<IDetourHookData>();
            set => SetCachedData(value);
        }

        public INativeDetour[] Detours
        {
            get => CachedData.Detours;
            set => CachedData = new()
            {
                Detours = value,
                Originals = CachedData.Originals,
                CallOriginalIdx = CachedData.CallOriginalIdx
            };
        }
        public TDelegate[] Originals
        {
            get => CachedData.Originals;
            set => CachedData = new()
            {
                Detours = CachedData.Detours,
                Originals = value,
                CallOriginalIdx = CachedData.CallOriginalIdx
            };
        }
        public int CallOriginalIdx
        {
            get => CachedData.CallOriginalIdx;
            set => CachedData = new()
            {
                Detours = CachedData.Detours,
                Originals = CachedData.Originals,
                CallOriginalIdx = value
            };
        }

        public IEnumerable<IntPtr> GetHookTargets();
        public TDelegate GetHookFunction();

        public void OnApplyHook() { }
    }

    public partial interface IVTableHook<TDelegate> : IGenericTask where TDelegate : Delegate
    {
        public new IVTableHookData CachedData
        {
            get => GetCachedData<IVTableHookData>();
            set => SetCachedData(value);
        }

        public IVTableHookData GetVTableHookInfo();

        public void OnApplyHook() { }
    }

    public partial interface IInitHarmony : IInitTask
    {
        public MethodBase GetOriginal();
        public Delegate? GetPrefix() => null;
        public Delegate? GetPostfix() => null;
        public Delegate? GetTranspiler() => null;
        public Delegate? GetFinalizer() => null;
        public Delegate? GetILManipulator() => null;
    }
    #endregion

    #region 游戏事件
    /// <summary>
    /// 注册二创的接口
    /// </summary>
    public partial interface IModData { }

    public partial interface IGenericModData : IModData { }

    /// <summary>
    /// 二创植物
    /// </summary>
    /// <typeparam name="TBase">植物基类</typeparam>
    public partial interface IModPlant<TBase> : IGenericModData where TBase : Plant { }

    /// <summary>
    /// 植物事件
    /// </summary>
    public partial interface IPlantEvent
    {
        public int GetOrder() => Default;

        public void OnUpdate(Trigger trigger) { }
        public void OnFixedUpdate(Trigger trigger) { }
        public void DieEvent(Trigger trigger, Plant.DieReason reason = Plant.DieReason.Default) { }
        public void DieEventMustExecute(Trigger trigger, Plant.DieReason reason = Plant.DieReason.Default) { }
        public void BeforeSerialized(Trigger trigger, SavePlantData data) { }
        public void AfterDeserialized(Trigger trigger, SavePlantData data) { }
    }
    #endregion
}
