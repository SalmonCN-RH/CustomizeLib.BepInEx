using BepInEx.Unity.IL2CPP.Hook;
using CustomizeLib.BepInEx.Internal.Extensions;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Runtime;
using Il2CppInterop.Runtime.Runtime.VersionSpecific.MethodInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.Tools
{
    internal static unsafe class DetourTools
    {
        /// <summary>
        /// Il2Cpp 默认构造函数名
        /// </summary>
        internal const string Constructor = ".ctor";
        /// <summary>
        /// Il2Cpp 默认静态构造函数名
        /// </summary>
        internal const string StaticConstructor = ".cctor";

        private static List<INativeDetour> Detours { get; set; } = [];

        /// <summary>
        /// 创建并启用 <see cref="INativeDetour"/>
        /// </summary>
        /// <typeparam name="T">方法委托</typeparam>
        /// <param name="from">目标方法</param>
        /// <param name="to">hook方法</param>
        /// <param name="original">原方法</param>
        /// <returns>Detour实例</returns>
        internal static INativeDetour CreateAndApply<T>(nint from, T to, out T original) where T : Delegate
        {
            var detour = INativeDetour.CreateAndApply(from, to, out original);
            Detours.Add(detour);
            return detour;
        }

        /// <summary>
        /// 创建并启用 <see cref="INativeDetour"/>
        /// </summary>
        /// <typeparam name="T">方法委托</typeparam>
        /// <param name="from">目标方法</param>
        /// <param name="to">hook方法</param>
        /// <param name="original">原方法</param>
        /// <returns>Detour实例</returns>
        internal static INativeDetour CreateAndApply<T>(INativeMethodInfoStruct from, T to, out T original) where T : Delegate
        {
            var detour = INativeDetour.CreateAndApply(from.MethodPointer, to, out original);
            Detours.Add(detour);
            return detour;
        }

        /// <summary>
        /// 获取 Il2Cpp 的方法
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
        /// <param name="namespaze">命名空间</param>
        /// <param name="className">类名</param>
        /// <param name="isGeneric">泛型方法</param>
        /// <param name="methodName">方法名</param>
        /// <param name="returnTypeName">返回值名</param>
        /// <param name="argTypes">参数列表</param>
        /// <returns>方法结构体</returns>
        internal static INativeMethodInfoStruct GetMethod(string assemblyName, string namespaze, string className, bool isGeneric, string methodName, string returnTypeName, params string[] argTypes)
        {
            var clz = IL2CPP.GetIl2CppClass(assemblyName, namespaze, className);
            IL2CPP.il2cpp_runtime_class_init(clz);
            return UnityVersionHandler.Wrap((Il2CppMethodInfo*)IL2CPP.GetIl2CppMethod(clz, isGeneric, methodName, returnTypeName, argTypes));
        }

        /// <summary>
        /// 获取 Il2Cpp 的方法
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="method">方法</param>
        /// <returns>方法结构体</returns>
        internal static INativeMethodInfoStruct GetMethod(Type type, MethodBase method)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));

            var argTypes = new List<string>();
            // 准备参数列表
            foreach (var parameter in method.GetParameters())
            {
                bool addr = parameter.IsOut || parameter.ParameterType.IsByRef;
                var paramType = parameter.ParameterType.IsByRef ? parameter.ParameterType.GetElementType() : parameter.ParameterType;
                argTypes.Add(paramType!.Il2CppName(addr));
            }
            var ret = method is MethodInfo info ? info.ReturnType.Il2CppName() : typeof(void).Il2CppName();
            return GetMethod($"{type.Assembly.GetName().Name}.dll", type.Namespace ?? "", type.Name, method.IsGenericMethod, method.Name, ret, [.. argTypes]);
        }

        /// <summary>
        /// 获取 Il2Cpp 的方法
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="methodName">方法</param>
        /// <returns>方法结构体</returns>
        internal static INativeMethodInfoStruct GetMethod(Type type, string methodName) =>
            GetMethod(type, type.GetMethod(methodName, InternalTools.TypeTools_DefaultDeclaredOnly)!);

        /// <summary>
        /// 获取 Il2Cpp 的构造函数
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="argTypes">参数</param>
        /// <returns>方法结构体</returns>
        internal static INativeMethodInfoStruct GetConstructor(Type type, params Type[] argTypes) =>
            GetMethod(type, type.GetConstructor(InternalTools.TypeTools_DefaultDeclaredOnly.RemoveStatic(), argTypes)!);

        /// <summary>
        /// 获取 Il2Cpp 的静态构造函数
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>方法结构体</returns>
        internal static INativeMethodInfoStruct GetStaticConstructor(Type type) =>
            GetMethod(type, type.TypeInitializer!);
    }
}
