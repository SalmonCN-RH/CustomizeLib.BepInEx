using Il2CppInterop.Runtime.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx
{
    public interface IVTableHook
    {

    }

    public partial interface IVTableHook<TDelegate>
    {
        void IModTask.Do()
        {
            var data = new IVTableHookData()
            {
                // Original = Marshal.GetDelegateForFunctionPointer<TDelegate>(GetHookTarget())
            };
            CachedData = data;
            OnApplyHook();
        }

        public struct IVTableHookData
        {
            public (TDelegate original, TDelegate hookFunction) Datas { get; set; }
        }

        public struct IVTableHookInfo
        {
            public (IVTableHookTarget target, TDelegate hookFunction) Hooks { get; set; }
        }

        public unsafe struct IVTableHookTarget
        {
            public Type TargetType { get; set; }
            public uint Slot { get; set; }

            public Il2CppClass* ClassStruct { get; set; }
        }
    }
}
