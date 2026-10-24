using Il2CppInterop.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx
{
    public static partial class Utils
    {
        public static class ClassPtrUtils
        {
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
        }
    }
}
