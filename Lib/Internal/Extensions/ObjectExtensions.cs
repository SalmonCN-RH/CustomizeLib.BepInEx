using CustomizeLib.BepInEx.Internal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.Extensions
{
    internal static class ObjectExtensions
    {
        /// <summary>
        /// 将 <paramref name="item"/> 转换为 <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="item">元素</param>
        /// <returns>转换后的<see cref="IEnumerable{T}"/></returns>
        internal static IEnumerable<T> ToIEnumerable<T>(this T item)
        {
            yield return item;
        }

        /// <summary>
        /// 将 <paramref name="obj"/> 转换为类型为 <typeparamref name="T"/> 的对象
        /// </summary>
        /// <typeparam name="T">转换类型</typeparam>
        /// <param name="obj">对象</param>
        /// <returns>转换后的对象</returns>
        internal static T To<T>(this object obj)
        {
            if (obj is T t) return t;
            return default!;
        }

        /// <summary>
        /// 判断 <paramref name="obj"/> 是否是 <paramref name="type"/> 的子类
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="type">基类</param>
        /// <param name="containBase">是否包含基类</param>
        /// <returns>是否是子类</returns>
        internal static bool IsSubTypeOf(this object obj, Type type, bool containBase = true) =>
            InternalTools.TypeTools_IsSubTypeOf(obj.GetType(), type, containBase);

        /// <summary>
        /// 判断 <paramref name="obj"/> 是否是 <typeparamref name="TBase"/> 的子类
        /// </summary>
        /// <typeparam name="TBase">基类</typeparam>
        /// <param name="obj">对象</param>
        /// <param name="containBase">是否包含基类</param>
        /// <returns>是否是子类</returns>
        internal static bool IsSubTypeOf<TBase>(this object obj, bool containBase = true) =>
            InternalTools.TypeTools_IsSubTypeOf(obj.GetType(), typeof(TBase), containBase);
    }
}
