using CustomizeLib.BepInEx.Internal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

// 设置 Logger
using Logger = CustomizeLib.BepInEx.Internal.Logger;

namespace CustomizeLib.BepInEx // 为了把多个partial代码组合在一起，需要调整命名空间
{
    #region 处理部分
    public partial interface IPlantEvent
    {
        /// <summary>
        /// 所有已注册的 Event
        /// </summary>
        internal static List<IPlantEvent> RegisterEvents { get; set; } = [];
        /// <summary>
        /// 绑定 IPlantEvent与对象
        /// </summary>
        internal static Internal.Auxiliary.TypeKeyDictionary<List<IPlantEvent>> BindingObjs { get; set; } = new();
        /// <summary>
        /// 对应Event触发的事件
        /// </summary>
        internal static Dictionary<EventType, Action<Trigger, object[]>> EventMaps { get; set; } = new()
        {
            [EventType.OnUpdate] = (trigger, _) => 
                Execute(trigger, EventType.OnUpdate, [], [], RunIfInGame),
            [EventType.OnUpdateMustExecute] = (trigger, _) =>
                Execute(trigger, EventType.OnUpdateMustExecute, [], [], RunAlways),
            [EventType.OnFixedUpdate] = (trigger, _) =>
                Execute(trigger, EventType.OnFixedUpdate, [], [], RunIfInGame),
            [EventType.OnFixedUpdateMustExecute] = (trigger, _) =>
                Execute(trigger, EventType.OnFixedUpdateMustExecute, [], [], RunAlways),
        };
        /// <summary>
        /// 对应Event触发的事件 (仅对单实例生效)
        /// </summary>
        internal static Dictionary<EventType, Action<IPlantEvent, Trigger, object[]>> EventSingleMaps { get; set; } = new()
        {
            [EventType.DieEvent] = (plantEvent, trigger, arr) => 
                ExecuteSingle(plantEvent, trigger, EventType.DieEvent, [typeof(Plant.DieReason)], arr, RunAlways),
            [EventType.DieEventMustExecute] = (plantEvent, trigger, arr) => 
                ExecuteSingle(plantEvent, trigger, EventType.DieEventMustExecute, [typeof(Plant.DieReason)], arr, RunAlways),
            [EventType.BeforeSerialized] = (plantEvent, trigger, arr) =>
                ExecuteSingle(plantEvent, trigger, EventType.BeforeSerialized, [typeof(SavePlantData)], arr, RunAlways),
            [EventType.AfterDeserialized] = (plantEvent, trigger, arr) =>
                ExecuteSingle(plantEvent, trigger, EventType.AfterDeserialized, [typeof(SavePlantData)], arr, RunAlways)
        };
        /// <summary>
        /// 缓存的某一类型的所有事件方法
        /// </summary>
        internal static Dictionary<Type, Dictionary<EventType, MethodInfo?>> CachedTypeMethods { get; set; } = [];
        /// <summary>
        /// 所有EventType列表
        /// </summary>
        internal static Lazy<List<EventType>> EventTypes { get; set; } = new(() => [.. Enum.GetValues<EventType>()]);
        /// <summary>
        /// EventType的名字列表
        /// </summary>
        internal static Lazy<List<string>> EventTypesName { get; set; } = new(() =>
        {
            var res = new List<string>();
            foreach (var type in EventTypes.Value)
                res.Add(type.ToString());
            return res;
        });
        /// <summary>
        /// 根据 string 反查 EventType
        /// </summary>
        internal static Lazy<Dictionary<string, EventType>> EventTypeNamesReverse { get; set; } = new(() =>
            EventTypesName.Value.ToDictionary(str => str, str => Enum.Parse<EventType>(str)));
        /// <summary>
        /// 缓存的方法与其触发器
        /// </summary>
        internal static Dictionary<MethodInfo, HashSet<Trigger>> CachedMethodTriggers { get; set; } = [];

        #region 静态方法
        /// <summary>
        /// 获取 <paramref name="obj"/> 上已绑定的 IPlantEvent
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>已绑定的 IPlantEvent</returns>
        internal static List<IPlantEvent> GetBindEvents(object obj) =>
            [.. BindingObjs.GetValueOrDefault(obj, []).OrderBy(i => i.GetOrder())]; // 按优先级排序

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="trigger">触发器</param>
        /// <param name="type">类型</param>
        /// <param name="extraArgs">额外参数类型</param>
        /// <param name="args">额外参数</param>
        internal static void Execute(Trigger trigger, EventType type, Type[] extraArgs, object[] args, Func<IPlantEvent, bool> canRun)
        {
            var cacheList = RegisterEvents.OrderBy(i => i.GetOrder()).ToList(); // 按优先级排序
            for (int i = 0; i < cacheList.Count; i++)
            {
                // cacheList[i] 为 null
                if (cacheList[i] == null)
                {
                    cacheList.RemoveAt(i);
                    continue;
                }
                // cacheList[i] 为 MonoBehaviour且未启用, 跳过但不移除 (可能还会启用)
                if (cacheList[i] is MonoBehaviour behaviour && !behaviour.isActiveAndEnabled) continue;
                if (canRun.Invoke(cacheList[i]))
                    ExecuteSingle(cacheList[i], trigger, type, extraArgs, args, canRun);
            }
            RegisterEvents = cacheList;
        }

        /// <summary>
        /// 执行事件 (单例)
        /// </summary>
        /// <param name="plantEvent">目标单例</param>
        /// <param name="trigger">触发器</param>
        /// <param name="type">类型</param>
        /// <param name="extraArgs">额外参数类型</param>
        /// <param name="args">额外参数</param>
        internal static void ExecuteSingle(IPlantEvent plantEvent, Trigger trigger, EventType type, Type[] extraArgs, object[] args, Func<IPlantEvent, bool> canRun)
        {
            if (plantEvent == null) return;
            var cachedEventType = plantEvent.GetType();
            InitCachedMethod(cachedEventType);

            // 搜索第一个参数为 Trigger, 且后续参数符合 extraArgs 的方法
            var method = CachedTypeMethods[cachedEventType][type];
            if (method == null) return;
            if (!canRun.Invoke(plantEvent)) return;
            if (!CheckTrigger(method, trigger)) return;
            method.Invoke(plantEvent, [trigger, .. args]);
        }

        /// <summary>
        /// 初始化缓存方法
        /// </summary>
        /// <param name="type">类型</param>
        private static void InitCachedMethod(Type type)
        {
            if (CachedTypeMethods.ContainsKey(type)) return;

            var value = new Dictionary<EventType, MethodInfo?>();

            // 初始化类里已有代码的 dictionary
            foreach (var method in type.FindMembers(MemberTypes.Method, InternalTools.TypeTools_DefaultDeclaredOnly, // 所有方法且仅在本类中定义
                (m, criteria) => ((List<string>)criteria!).Any(str => m.Name.EndsWith(str)), EventTypesName.Value).  // 所有方法结尾名称包含了 EventType 里任意一枚举值的 ToString
                Cast<MethodInfo>()) // 转为 MethodInfo
            {
                var methodName = method.Name[(method.Name.LastIndexOf('.') + 1)..]; // 从最后一个.(不含)到末尾
                if (!EventTypeNamesReverse.Value.TryGetValue(methodName, out var eventType)) continue; // 获取不到对应的就返回
                value.Add(eventType, method);
            }

            // 为代码里没有定义的 event 赋值，防止读取时提示键不存在
            foreach (var eventType in EventTypes.Value)
                if (!value.ContainsKey(eventType)) value[eventType] = null;

            CachedTypeMethods[type] = value;
        }

        /// <summary>
        /// 检查方法是否匹配此触发器
        /// </summary>
        /// <param name="info">方法</param>
        /// <param name="trigger">触发器</param>
        /// <returns>是否匹配</returns>
        internal static bool CheckTrigger(MethodInfo info, Trigger trigger)
        {
            // 如果不存在，则进行缓存
            if (!CachedMethodTriggers.TryGetValue(info, out var value))
            {
                var attrs = info.GetCustomAttributes<TriggerAttribute>(false);
                HashSet<Trigger> set = [.. attrs.Select(attr => attr.Trigger)];
                if (set.Count <= 0) set = [Trigger.Pre, Trigger.Post]; // 默认所有触发
                CachedMethodTriggers[info] = value = set;
            }
            return value.Contains(trigger);
        }

        /// <summary>
        /// 如果游戏状态是 InGame 则调用
        /// </summary>
        /// <returns>能否调用</returns>
        internal static bool RunIfInGame(IPlantEvent _) => GameAPP.theGameStatus == GameStatus.InGame;
        /// <summary>
        /// 总是触发调用
        /// </summary>
        /// <returns>true</returns>
        internal static bool RunAlways(IPlantEvent _) => true;
        #endregion

        /// <summary>
        /// 添加到列表
        /// </summary>
        public IPlantEvent Register()
        {
            if (RegisterEvents.Contains(this)) Logger.LogWarning($"You are registering a {GetType()} instance that has already been registered. Are you sure?");
            RegisterEvents.Add(this);
            return this;
        }

        /// <summary>
        /// 从列表中移除
        /// </summary>
        public IPlantEvent Unregister()
        {
            RegisterEvents.Remove(this);
            return this;
        }

        /// <summary>
        /// 将本 IPlantEvent 与 <paramref name="obj"/> 绑定
        /// </summary>
        /// <param name="obj">绑定对象</param>
        public void Bind(object obj)
        {
            if (obj == null) return;
            var list = BindingObjs.GetValueOrDefault(obj, []);
            list.Add(this);
            BindingObjs.SetValue(obj, list);
        }

        internal enum EventType
        {
            OnUpdate,
            OnUpdateMustExecute,
            OnFixedUpdate,
            OnFixedUpdateMustExecute,
            DieEvent,
            DieEventMustExecute,
            BeforeSerialized,
            AfterDeserialized
        }
    }
    #endregion
}
