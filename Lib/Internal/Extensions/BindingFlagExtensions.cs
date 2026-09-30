using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.Extensions
{
    internal static class BindingFlagExtensions
    {
        /// <summary>
        /// 为 <paramref name="flags"/> 添加 <paramref name="add"/> 的 <see cref="BindingFlags"/>
        /// </summary>
        /// <param name="flags">原标志</param>
        /// <param name="add">添加标志</param>
        /// <returns>添加后的 <see cref="BindingFlags"/></returns>
        internal static BindingFlags AddFlags(this BindingFlags flags, BindingFlags add) => flags | add;

        /// <summary>
        /// 为 <paramref name="flags"/> 移除 <paramref name="remove"/> 的 <see cref="BindingFlags"/>
        /// </summary>
        /// <param name="flags">原标志</param>
        /// <param name="remove">移除标志</param>
        /// <returns>移除后的 <see cref="BindingFlags"/></returns>
        internal static BindingFlags RemoveFlags(this BindingFlags flags, BindingFlags remove) => flags & ~remove;

        // 为BindingFlags添加过滤器
        internal static BindingFlags AddPublic(this BindingFlags flags) => flags | BindingFlags.Public;
        internal static BindingFlags AddNonPublic(this BindingFlags flags) => flags | BindingFlags.NonPublic;
        internal static BindingFlags AddStatic(this BindingFlags flags) => flags | BindingFlags.Static;
        internal static BindingFlags AddInstance(this BindingFlags flags) => flags | BindingFlags.Instance;
        internal static BindingFlags AddDeclaredOnly(this BindingFlags flags) => flags | BindingFlags.DeclaredOnly;

        // 为BindingFlags移除过滤器
        internal static BindingFlags RemovePublic(this BindingFlags flags) => flags & ~BindingFlags.Public;
        internal static BindingFlags RemoveNonPublic(this BindingFlags flags) => flags & ~BindingFlags.NonPublic;
        internal static BindingFlags RemoveStatic(this BindingFlags flags) => flags & ~BindingFlags.Static;
        internal static BindingFlags RemoveInstance(this BindingFlags flags) => flags & ~BindingFlags.Instance;
        internal static BindingFlags RemoveDeclaredOnly(this BindingFlags flags) => flags & ~BindingFlags.DeclaredOnly;
    }
}
