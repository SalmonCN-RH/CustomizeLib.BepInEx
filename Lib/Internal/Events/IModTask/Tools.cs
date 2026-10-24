using CustomizeLib.BepInEx.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx
{
    public partial interface IModTask
    {
        #region 回调数据
        internal static Dictionary<Type, List<ITaskCallback>> Callbacks { get; set; } = [];
        #endregion

        #region 缓存数据
        internal static Dictionary<Type, object> CachedDatas = [];
        #endregion

        protected static IEnumerable<T> GetAllTask<T>() where T : IModTask
        {
            return Utils.TypeUtils.GetAllDerivedTypes<T>(Utils.TypeUtils.GetAllAssemblies(), false).Select(t =>
            {
                if (t.IsGenericType) return default!;
                if (t.IsInterface) return default!;
                return (T)Activator.CreateInstance(t)!;
            }).Where(task => task != null).OrderBy(task => task.GetOrder());
        }

        protected static void DoAll<T>() where T : IModTask
        {
            foreach (var task in GetAllTask<T>()) task.Do();
        }

        #region 获取 callback
        /// <summary>
        /// 获取 <paramref name="type"/> 下的所有 callback
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>callback 列表</returns>
        internal static IEnumerable<ITaskCallback> GetAllCallback(Type type) => Callbacks.GetValueOrDefault(type, []);
        /// <summary>
        /// 获取 <typeparamref name="T"/> 下的所有 callback
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>callback 列表</returns>
        internal static IEnumerable<ITaskCallback> GetAllCallback<T>() where T : IModTask => GetAllCallback(typeof(T));
        #endregion

        #region 添加 callback
        /// <summary>
        /// 向 <paramref name="type"/> 里添加 <paramref name="callback"/>
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="callback">目标回调</param>
        public static void AddCallback(Type type, ITaskCallback callback) => Callbacks[type] = [.. Callbacks.GetValueOrDefault(type, []), callback];
        /// <summary>
        /// 向 <typeparamref name="T"/> 里添加 <paramref name="callback"/>
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="callback">目标回调</param>
        public static void AddCallback<T>(ITaskCallback callback) => AddCallback(typeof(T), callback);
        #endregion

        #region 执行 callback
        /// <summary>
        /// 以参数 <paramref name="args"/> 执行 <paramref name="type"/> 下的所有 callback
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="args">参数</param>
        internal static void RunAllCallback(Type type, object?[] args) => Callbacks.GetValueOrDefault(type, []).DoAll(callback => callback.Execute(args));

        /// <summary>
        /// 以参数 <paramref name="args"/> 执行 <typeparamref name="T"/> 下的所有 callback
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="args">参数</param>
        internal static void RunAllCallback<T>(object?[] args) where T : IModTask => RunAllCallback(typeof(T), args);
        #endregion
    }
}
