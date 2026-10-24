using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

// 设置 Logger
using Logger = CustomizeLib.BepInEx.Internal.Logger;

namespace CustomizeLib.BepInEx // 为了把多个partial代码组合在一起，需要调整命名空间
{
    #region 处理部分
    public partial interface IPlantEvent<TBase>
    {
        public void Bind(TBase obj) => IPlantEventHelper.BindEvents.SetValue(obj, [.. IPlantEventHelper.BindEvents.GetValueOrDefault(obj, []), this]);
    }

    internal static class IPlantEventHelper
    {
        /// <summary>
        /// 缓存的 Event 方法 ((类型, 方法名), 调用方法)
        /// </summary>
        private static Dictionary<(Type type, string name), MethodInfo> CachedTypeMethods { get; set; } = new();
        /// <summary>
        /// 绑定对象的 Event
        /// </summary>
        internal static TypeKeyDictionary<HashSet<IPlantEventBase>> BindEvents { get; set; } = new();

        /// <summary>
        /// 初始化数据和hook
        /// </summary>
        internal static void Initialize()
        {
            // 支持调用的事件名称
            var eventNames = typeof(IPlantEvent<>).GetMethods(Utils.TypeUtils.DefaultDeclaredOnly).Select(m => GetRealMethodName(m.Name)).ToHashSet();
            // 手动应用hook方法
            var onEvent = typeof(IGameAppEvent).GetMethod(nameof(IGameAppEvent.OnEvent), Utils.TypeUtils.DefaultFlag)!;

            foreach (var type in Utils.TypeUtils.GetAllTypes())
            {
                if (!Utils.TypeUtils.IsSubTypeOf(type, typeof(IPlantEvent<>), false)) continue; // 只处理 IPlantEvent<> 的子类

                if (type.GenericTypeArguments.Length <= 0) continue;
                var genericType = type.GenericTypeArguments[0];

                type.GetMethods(Utils.TypeUtils.DefaultDeclaredOnly).DoAll(m =>
                {
                    if (!eventNames.Contains(GetRealMethodName(m.Name))) return; // 如果没有任何一个方法匹配名称则跳过

                    InitializeMethod(type, m); // 缓存方法

                    var (delegateType, hookFunction) = TryGetDelegate(m.Name);
                    var hookType = typeof(PlantVirtualMethodHook<>).MakeGenericType(delegateType);
                    var cons = hookType.GetConstructor(Utils.TypeUtils.DefaultDeclaredOnly, [delegateType, typeof(string), typeof(Type)])!;
                    var instance = cons.Invoke([hookFunction, m.Name, type]);

                    onEvent.Invoke(instance, []);
                });
            }
        }

        private static void InitializeMethod(Type type, MethodInfo method) =>
            CachedTypeMethods[(type, GetRealMethodName(method.Name))] = method;

        private static (Type delegateType, Delegate function) TryGetDelegate(string name)
        {
            switch (name)
            {
                case nameof(IPlantEvent<>.OnUpdate):
                    break;
            }

            throw new ArgumentException($"Unsupported function name {name}");
        }

        private static void CallBindEvents<TDelegate>(PlantVirtualMethodHook<TDelegate> self, object obj, string methodName, object[] args) where TDelegate : Delegate
        {
            var events = BindEvents.GetValueOrDefault(obj, []);
            var invokeList = new List<(MethodInfo m, IPlantEventBase plantEvent)>();

            foreach (var plantEvent in events)
            {

            }

            self.CallCurrent(args); // 调用原函数
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetRealMethodName(string name) => name[(name.LastIndexOf('.') + 1)..];

        private static void RunAll<TDelegate>(PlantVirtualMethodHook<TDelegate> hook) where TDelegate : Delegate
        {
            hook.CallCurrent([]);
        }

        internal delegate void OnUpdateDelegate(IntPtr plant, IntPtr method);
    }

    internal sealed class PlantVirtualMethodHook<TDelegate> : IVTableHook<TDelegate> where TDelegate : Delegate
    {
        private TDelegate HookFunction { get; set; }
        private string TargetMethod { get; set; }
        private Type TargetType { get; set; }

        internal PlantVirtualMethodHook(TDelegate hookFunction, string targetMethod, Type type)
        {
            HookFunction = hookFunction;
            TargetMethod = targetMethod;
            TargetType = type;
        }

        public TDelegate GetHookFunction() => HookFunction;

        public VTableHookTarget[] GetHookTargets() =>
            [
                new(TargetType, Utils.DetourUtils.GetMethod(TargetType, TargetMethod).Slot)
            ];
    }
    #endregion
}
