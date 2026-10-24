using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Runtime;
using Il2CppInterop.Runtime.Runtime.VersionSpecific.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx
{
    public static partial class Utils
    {
        public static class NativeClassUtils
        {
            /// <summary>
            /// 获取 <paramref name="type"/> 的 Il2CppClass 结构体
            /// </summary>
            /// <param name="type">类型</param>
            /// <returns>结构体</returns>
            public static unsafe INativeClassStruct GetClassStructUnsafe(Type type)
            {
                var ptr = IL2CPP.GetIl2CppClass($"{type.Assembly}.dll", type.Namespace ?? "", type.Name);
                IL2CPP.il2cpp_runtime_class_init(ptr);
                return UnityVersionHandler.Wrap((Il2CppClass*)ptr);
            }

            /// <summary>
            /// 获取 <paramref name="type"/> 的 Il2CppClass 结构体
            /// </summary>
            /// <param name="type">类型</param>
            /// <returns>结构体</returns>
            public static INativeClassStruct GetClassStruct(Type type) => GetClassStructUnsafe(type);

            /// <summary>
            /// 获取 <typeparamref name="T"/> 的 Il2CppClass 结构体
            /// </summary>
            /// <typeparam name="T">类型</typeparam>
            /// <returns>结构体</returns>
            public static INativeClassStruct GetClassStruct<T>() => GetClassStructUnsafe(typeof(T));

            /// <summary>
            /// 获取 Il2Cpp 类 <paramref name="type"/> 的 NativeClassPtr
            /// </summary>
            /// <param name="type">类型</param>
            /// <returns>指针</returns>
            public static IntPtr GetClassPtr(Type type)
            {
                var ptr = IL2CPP.GetIl2CppClass($"{type.Assembly}.dll", type.Namespace ?? "", type.Name);
                IL2CPP.il2cpp_runtime_class_init(ptr);
                return ptr;
            }

            /// <summary>
            /// 获取 Il2Cpp 类 <typeparamref name="T"/> 的 NativeClassPtr
            /// </summary>
            /// <typeparam name="T">类型</typeparam>
            /// <returns>指针</returns>
            public static IntPtr GetClassPtr<T>() => GetClassPtr(typeof(T));

            /// <summary>
            /// 通过 <see cref="Il2CppClassPointerStore"/> 获取 Il2Cpp 类 <paramref name="type"/> 的 NativeClassPtr
            /// </summary>
            /// <param name="type">类型</param>
            /// <returns>指针</returns>
            public static IntPtr GetClassPtrByStore(Type type) => Il2CppClassPointerStore.GetNativeClassPointer(type);

            /// <summary>
            /// 通过 <see cref="Il2CppClassPointerStore{T}"/> 获取 Il2Cpp 类 <typeparamref name="T"/> 的 NativeClassPtr
            /// </summary>
            /// <typeparam name="T">类型</typeparam>
            /// <returns>指针</returns>
            public static IntPtr GetClassPtrByStore<T>() => Il2CppClassPointerStore<T>.NativeClassPtr;

            /// <summary>
            /// 初始化类型
            /// </summary>
            /// <param name="type">类型</param>
            public static void Initialize(Type type)
            {
                var ptr = IL2CPP.GetIl2CppClass($"{type.Assembly.GetName().Name}.dll", type.Namespace ?? "", type.Name);
                LibLogger.LogInfo($"{type.Assembly.GetName().Name}.dll {type.Namespace ?? ""} {type.Name} {ptr}");
                IL2CPP.il2cpp_runtime_class_init(ptr);
            }

            /// <summary>
            /// 初始化类型
            /// </summary>
            /// <typeparam name="T">类型</typeparam>
            public static void Initialize<T>() => Initialize(typeof(T));
        }
    }
}
