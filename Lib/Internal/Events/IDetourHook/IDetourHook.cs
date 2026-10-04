using BepInEx.Unity.IL2CPP.Hook;
using CustomizeLib.BepInEx.Internal.Tools;
using Il2CppInterop.Runtime.Runtime;
using Il2CppInterop.Runtime.Runtime.VersionSpecific.MethodInfo;
using System.Reflection;

namespace CustomizeLib.BepInEx
{
    public partial interface IDetourHook<TDelegate>
    {
        void IModTask.Do()
        {
            var data = new DetourHookData
            {
                Detour = INativeDetour.CreateAndApply(GetHookTarget().MethodPointer, GetHookFunction(), out var original),
                Original = original
            };
            CachedData = data;

            OnApplyHook();
        }

        public struct DetourHookData
        {
            public INativeDetour Detour { get; set; }
            public TDelegate Original { get; set; }
        }

        public unsafe struct DetourHookTarget(IntPtr methodPointer)
        {
            // 直接设置 MethodPointer
            internal IntPtr MethodPointer { get; set; } = methodPointer;

            public DetourHookTarget(INativeMethodInfoStruct methodInfo) : this(methodInfo.MethodPointer) { }
            public DetourHookTarget(Il2CppMethodInfo* methodInfo) : this(UnityVersionHandler.Wrap(methodInfo)) { }
            public DetourHookTarget(string assemblyName, string namespaze, string className, bool isGeneric, string methodName, string returnTypeName, params string[] argTypes) :
                this(InternalTools.DetourTools_GetMethod(assemblyName, namespaze, className, isGeneric, methodName, returnTypeName, argTypes)) { }
            public DetourHookTarget(Type type, MethodBase method) : this(InternalTools.DetourTools_GetMethod(type, method)) { }
            public DetourHookTarget(MethodBase method) : this(InternalTools.DetourTools_GetMethod(method)) { }
            public DetourHookTarget(Type type, string methodName, Type[] ctorParams = null!) : this(
                methodName == ".cctor" ? InternalTools.DetourTools_GetStaticConstructor(type) : // .cctor -> 静态函数
                    (methodName == ".ctor" ? InternalTools.DetourTools_GetConstructor(type, ctorParams ?? []) : // .ctor -> 构造函数
                        InternalTools.DetourTools_GetMethod(type, methodName))) { } // 其他 -> 普通函数
        }
    }
}
