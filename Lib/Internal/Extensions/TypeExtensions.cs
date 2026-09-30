using Il2CppInterop.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.Extensions
{
    internal static class TypeExtensions
    {
        /// <summary>
        /// 获取 <paramref name="type"/> 在 Il2Cpp 中的名称
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="addr">指针标记</param>
        /// <returns>名称</returns>
        internal static string Il2CppName(this Type type, bool addr = false) => IL2CPP.RenderTypeName(type, addr);

        /// <summary>
        /// 获取 <paramref name="type"/> 在 Il2Cpp 中的对应类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>Il2Cpp 类型</returns>
        internal static Il2CppSystem.Type Il2CppType(this Type type) => Il2CppInterop.Runtime.Il2CppType.From(type);
    }
}
