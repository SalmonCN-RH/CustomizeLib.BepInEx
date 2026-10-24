using BepInEx.Unity.IL2CPP.Hook;
using CustomizeLib.BepInEx.Internal;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx
{
    #region Mod事件
    /// <summary>
    /// 所有 Task 的基类
    /// </summary>
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
        public int GetOrder() => CuLibPriority.Default;

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

    /// <summary>
    /// 当 Lib 执行时执行
    /// </summary>
    public partial interface ILoadTask : IModTask 
    {
        public void OnLoad();
        void IModTask.Do() => OnLoad();
    }

    /// <summary>
    /// 当 GameAPP 执行指定方法时执行
    /// </summary>
    public partial interface IGameAppEvent : IModTask
    {
        public GameAppEvent Filter => GameAppEvent.Default;

        public void OnEvent();
        void IModTask.Do() => OnEvent();
    }

    /// <summary>
    /// 在所有一般插件加载前执行
    /// </summary>
    public partial interface IInitTask : IModTask 
    {
        public void OnInit();
        void IModTask.Do() => OnInit();
    }

    /// <summary>
    /// 基于 INativeDetour 的 Hook
    /// </summary>
    /// <typeparam name="TDelegate">函数签名</typeparam>
    public partial interface IDetourHook<TDelegate> : IGenericTask where TDelegate : Delegate
    {
        public new DetourHookData<TDelegate> CachedData
        {
            get => GetCachedData<DetourHookData<TDelegate>>();
            set => SetCachedData(value);
        }

        public INativeDetour Detour
        {
            get => CachedData.Detour;
            set => CachedData = new()
            {
                Detour = value,
                Original = CachedData.Original
            };
        }
        public TDelegate Original
        {
            get => CachedData.Original;
            set => CachedData = new()
            {
                Detour = CachedData.Detour,
                Original = value,
            };
        }
        public DetourHookTarget GetHookTarget();
        public TDelegate GetHookFunction();

        public void OnApplyHook() { }
    }

    /// <summary>
    /// 基于 自实现虚函数表Hook 的 Hook
    /// </summary>
    /// <typeparam name="TDelegate">函数签名</typeparam>
    public partial interface IVTableHook<TDelegate> : IGameAppEvent where TDelegate : Delegate
    {
        public new VTableHookData<TDelegate> CachedData
        {
            get => GetCachedData<VTableHookData<TDelegate>>();
            set => SetCachedData(value);
        }

        public TDelegate[] Originals
        {
            get => CachedData.Originals;
            set => CachedData = new()
            {
                Originals = value,
                CallingIdx = CachedData.CallingIdx
            };
        }
        public uint CallingIdx
        {
            get => CachedData.CallingIdx;
            set => CachedData = new()
            {
                Originals = CachedData.Originals,
                CallingIdx = value
            };
        }

        public VTableHookTarget[] GetHookTargets();
        public TDelegate GetHookFunction();

        public void OnApplyHook() { }
    }
    
    /// <summary>
    /// 在所有一般插件加载前运行的 HarmonyPatch
    /// </summary>
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
    public partial interface IModPlant<TBase> : IGenericModData where TBase : Plant 
    {
        public CustomPlant CustomPlant { get; }
    }

    public interface IPlantEventBase { }

    /// <summary>
    /// 植物事件
    /// </summary>
    public partial interface IPlantEvent<TBase> : IPlantEventBase where TBase : Plant
    {
        public int GetOrder() => CuLibPriority.Default;

        public void OnUpdate() { }
        public void OnFixedUpdate() { }
        public void DieEvent(Plant.DieReason reason = Plant.DieReason.Default) { }
        public void DieEventMustExecute(Plant.DieReason reason = Plant.DieReason.Default) { }
        public void BeforeSerialized(SavePlantData data) { }
        public void AfterDeserialized(SavePlantData data) { }
    }
    #endregion

    #region 实现接口
    /// <summary>
    /// ModTask 的 callback 基类
    /// </summary>
    public partial interface ITaskCallback
    {
        public void Execute(object?[] args);
    }
    #endregion
}
