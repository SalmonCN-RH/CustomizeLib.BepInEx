using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx
{
    public static partial class Extensions
    {
        /// <summary>
        /// 获取 <paramref name="hook"/> 的 Originals
        /// </summary>
        /// <typeparam name="TDelegate">委托类型</typeparam>
        /// <param name="hook"><see cref="IVTableHook{TDelegate}"/> 实例</param>
        /// <returns>实例的 Originals</returns>
        public static TDelegate[] GetOriginals<TDelegate>(this IVTableHook<TDelegate> hook) where TDelegate : Delegate =>
            hook.Originals;

        /// <summary>
        /// 获取 <paramref name="hook"/> 的 CallingIdx
        /// </summary>
        /// <typeparam name="TDelegate">委托类型</typeparam>
        /// <param name="hook"><see cref="IVTableHook{TDelegate}"/> 实例</param>
        /// <returns>实例的 CallingIdx</returns>
        public static uint GetCallingIdx<TDelegate>(this IVTableHook<TDelegate> hook) where TDelegate : Delegate =>
            hook.CallingIdx;

        /// <summary>
        /// 调用 <paramref name="hook"/> 当前 Hook 的原函数
        /// </summary>
        /// <typeparam name="TDelegate">委托类型</typeparam>
        /// <param name="hook"><see cref="IVTableHook{TDelegate}"/> 实例</param>
        /// <param name="param">调用参数</param>
        /// <returns>原函数的返回值</returns>
        public static object? CallCurrent<TDelegate>(this IVTableHook<TDelegate> hook, object[] param) where TDelegate : Delegate =>
            hook.GetOriginals()[hook.GetCallingIdx()].DynamicInvoke(param);
    }
}
