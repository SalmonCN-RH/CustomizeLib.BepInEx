using BepInEx.Unity.IL2CPP.Hook;
using CustomizeLib.BepInEx.Internal;
using CustomizeLib.BepInEx.Internal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace CustomizeLib.BepInEx
{
    public interface IDetourHook
    {
        internal static bool CachedEnvReady { get; set; } = false;
        public static bool EnvReady => CachedEnvReady;
    }

    public partial interface IDetourHook<TDelegate>
    {
        public bool EnvReady => IDetourHook.EnvReady;

        void IModTask.Do()
        {
            var targets = GetHookTargets();

            var detours = new List<INativeDetour>();
            var originals = new List<TDelegate>();
            int idx = 0;

            foreach (var target in targets)
            {
                detours.Add(InternalTools.DetourTools_CreateAndApply(target, Wrap(idx++), out var original));
                originals.Add(original);
                Logger.LogInfo($"handle {target:X} {Marshal.GetFunctionPointerForDelegate(original):X}");
            }

            var data = new IDetourHookData
            {
                Detours = [.. detours],
                Originals = [.. originals]
            };
            CachedData = data;

            OnApplyHook();
        }

        internal TDelegate Wrap(int idx)
        {
            var invoke = typeof(TDelegate).GetMethod("Invoke") ?? throw new InvalidOperationException($"{typeof(TDelegate)} is not a delegate type");
            var parameters = invoke.GetParameters();
            var paramTypes = new Type[parameters.Length + 1];
            paramTypes[0] = typeof(object);
            for (int i = 0; i < parameters.Length; ++i)
                paramTypes[i + 1] = parameters[i].ParameterType;

            var dynamicMethod = new DynamicMethod(
                name: $"IDetourHook<{typeof(TDelegate).Name}>_Wrap_DynamicMethod_{idx}",
                returnType: invoke.ReturnType,
                parameterTypes: paramTypes,
                m: typeof(IDetourHook<>).Module,
                skipVisibility: true);

            var il = dynamicMethod.GetILGenerator();

            // 获取 IDetourHook<TDelegate> 类型
            var interfaceType = typeof(IDetourHook<>).MakeGenericType(typeof(TDelegate));

            // 获取 CallOriginalIdx 属性
            var idxProp = interfaceType.GetProperty(nameof(CallOriginalIdx), InternalTools.TypeTools_DefaultFlag)!;
            var setter = idxProp.GetSetMethod()!;

            // 获取 GetHookFunction 方法
            var getHookFunction = interfaceType.GetMethod(nameof(GetHookFunction), InternalTools.TypeTools_DefaultFlag)!;

            static void EmitInt(ILGenerator il, int value)
            {
                switch (value)
                {
                    case -1: il.Emit(OpCodes.Ldc_I4_M1); break;
                    case 0: il.Emit(OpCodes.Ldc_I4_0); break;
                    case 1: il.Emit(OpCodes.Ldc_I4_1); break;
                    case 2: il.Emit(OpCodes.Ldc_I4_2); break;
                    case 3: il.Emit(OpCodes.Ldc_I4_3); break;
                    case 4: il.Emit(OpCodes.Ldc_I4_4); break;
                    case 5: il.Emit(OpCodes.Ldc_I4_5); break;
                    case 6: il.Emit(OpCodes.Ldc_I4_6); break;
                    case 7: il.Emit(OpCodes.Ldc_I4_7); break;
                    case 8: il.Emit(OpCodes.Ldc_I4_8); break;
                    default:
                        if (value >= sbyte.MinValue && value <= sbyte.MaxValue)
                            il.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
                        else
                            il.Emit(OpCodes.Ldc_I4, value);
                        break;
                }
            }

            static void EmitLdarg(ILGenerator il, int index)
            {
                switch (index)
                {
                    case 0: il.Emit(OpCodes.Ldarg_0); break;
                    case 1: il.Emit(OpCodes.Ldarg_1); break;
                    case 2: il.Emit(OpCodes.Ldarg_2); break;
                    case 3: il.Emit(OpCodes.Ldarg_3); break;
                    default:
                        if (index <= byte.MaxValue)
                            il.Emit(OpCodes.Ldarg_S, (byte)index);
                        else
                            il.Emit(OpCodes.Ldarg, index);
                        break;
                }
            }

            // this.CallOriginalIdx = idx;
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, interfaceType);
            EmitInt(il, idx);
            il.Emit(OpCodes.Callvirt, setter);

            // this.GetHookFunction()
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, interfaceType);
            il.Emit(OpCodes.Callvirt, getHookFunction);

            // 加载原始参数, 索引从 1 开始，因为 0 是 target
            for (int i = 0; i < parameters.Length; i++) EmitLdarg(il, i + 1);

            // 调用委托的 Invoke
            il.Emit(OpCodes.Callvirt, invoke);

            // return;
            il.Emit(OpCodes.Ret);

            // 把 DynamicMethod 的第一个 object 参数绑定为 target
            return (TDelegate)dynamicMethod.CreateDelegate(typeof(TDelegate), this);
        }

        public struct IDetourHookData
        {
            public INativeDetour[] Detours { get; set; }
            public TDelegate[] Originals { get; set; }
            public int CallOriginalIdx { get; set; }
        }
    }
}
