using CustomizeLib.BepInEx.Internal.Tools;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Runtime;
using Il2CppInterop.Runtime.Runtime.VersionSpecific.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx
{
    public unsafe partial interface IVTableHook<TDelegate>
    {
        /// <summary>
        /// 让生成的委托不要被 GC 回收
        /// </summary>
        private static List<(TDelegate original, TDelegate hook, IntPtr wrapMethod)> GCKeepAlive { get; set; } = [];

        GameAppEvent IGameAppEvent.Filter => GameAppEvent.PostAwake;

        void IGameAppEvent.OnEvent()
        {
            var targets = GetHookTargets();
            var originals = new TDelegate[targets.Length];

            for (uint i = 0; i < targets.Length; ++i)
            {
                var target = targets[i];

                var vtable = (VirtualInvokeData*)target.VTablePointer;
                var strc = vtable[target.Slot];

                // 备份 original
                var original = Marshal.GetDelegateForFunctionPointer<TDelegate>(strc.methodPtr);
                originals[i] = original;

                var (wrapperPtr, wrapper) = Wrap(i); // 获取包装后的函数指针
                GCKeepAlive.Add((original, wrapper, wrapperPtr));

                // 替换
                // 处理 VirtualInvokeData.method
                var info = UnityVersionHandler.Wrap(strc.method);
                info.MethodPointer = info.VirtualMethodPointer = wrapperPtr;
                strc.method = info.MethodInfoPointer; // 把新的 method 赋回去

                // 处理 VirtualInvokeData.methodPtr
                strc.methodPtr = wrapperPtr;

                vtable[target.Slot] = strc;
            }

            CachedData = new()
            {
                Originals = originals,
            };
            OnApplyHook();
        }

        internal (IntPtr wrapperPtr, TDelegate wrapper) Wrap(uint idx)
        {
            var invoke = typeof(TDelegate).GetMethod("Invoke", InternalTools.TypeTools_DefaultFlag) ?? throw new ArgumentException($"Can't find the Invoke method in TDelegate");
            var parameters = invoke.GetParameters();
            var returnType = invoke.ReturnType;

            // 复制参数
            var paramTypes = new Type[parameters.Length + 1];
            paramTypes[0] = typeof(IVTableHook<TDelegate>);
            for (int i = 0; i < parameters.Length; ++i)
                paramTypes[i + 1] = parameters[i].ParameterType;

            var dm = new DynamicMethod(
                name: $"DynamicWrapper_{typeof(TDelegate).Name}_{idx}",
                returnType: returnType,
                parameterTypes: paramTypes,
                owner: GetType(), // 不能使用接口类作为 owner 参数
                skipVisibility: true);
            var il = dm.GetILGenerator();

            // this.CallingIdx = idx;
            var setter = typeof(IVTableHook<TDelegate>).GetProperty(nameof(CallingIdx), InternalTools.TypeTools_DefaultFlag)!.GetSetMethod() ??
                throw new ArgumentException($"Can't find the setter for CallingIdx");
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldc_I4, unchecked((int)idx));
            il.Emit(OpCodes.Callvirt, setter);

            // return this.GetDelegate().Invoke(arg0, arg1, arg2, ...);
            var getHookFunction = typeof(IVTableHook<TDelegate>).GetMethod(nameof(GetHookFunction), InternalTools.TypeTools_DefaultFlag)!;
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Callvirt, getHookFunction);

            for (int i = 0; i < parameters.Length; ++i)
                il.Emit(OpCodes.Ldarg, i + 1);
            il.Emit(OpCodes.Callvirt, invoke);
            il.Emit(OpCodes.Ret);

            var d = dm.CreateDelegate<TDelegate>(this);
            return (Marshal.GetFunctionPointerForDelegate(d), d);
        }

        public struct VTableHookData
        {
            public TDelegate[] Originals { get; set; }
            public uint CallingIdx { get; set; }
        }

        public unsafe struct VTableHookTarget(IntPtr vTablePointer, ushort slot)
        {
            internal IntPtr VTablePointer { get; set; } = vTablePointer;
            internal ushort Slot { get; set; } = slot;

            public VTableHookTarget(VirtualInvokeData* vtable, ushort slot) : this((IntPtr)vtable, slot) { }
            public VTableHookTarget(INativeClassStruct classStruct, ushort slot) : this(classStruct.VTable, slot) { }
            public VTableHookTarget(Il2CppClass* clazz, ushort slot) : this(UnityVersionHandler.Wrap(clazz), slot) { }
            public VTableHookTarget(Type type, ushort slot) : this((Il2CppClass*)Il2CppClassPointerStore.GetNativeClassPointer(type), slot) { }
            public VTableHookTarget(string asmNameWithDll, string namespaze, string name, ushort slot) : this((Il2CppClass*)IL2CPP.GetIl2CppClass(asmNameWithDll, namespaze, name), slot) { }
            public VTableHookTarget(Il2CppSystem.Type type, ushort slot) : this($"{type.Assembly.GetName().Name}.dll", type.Name, type.Name, slot) { }
        }
    }
}
