using BepInEx;
using BepInEx.Unity.IL2CPP;
using BepInEx.Unity.IL2CPP.Hook;
using CustomizeLib.BepInEx.Internal.Auxiliary;
using CustomizeLib.BepInEx.Internal.Extensions.EventExtensions;
using CustomizeLib.BepInEx.Internal.Tools;
using Cysharp.Threading.Tasks;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Runtime;
using Il2CppInterop.Runtime.Runtime.VersionSpecific.Class;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace CustomizeLib.BepInEx.Internal.Test
{
    internal struct FeatureTest_Load : ILoadTask
    {
        public delegate void AttributeEventDelegate(IntPtr @this, IntPtr method);
        public static AttributeEventDelegate AttributeEvent;
        public static void CustomAttributeEvent(IntPtr @this, IntPtr method)
        {
            AttributeEvent.Invoke(@this, method);
            Logger.LogInfo($"custom attr event {method:X}");
        }
        public static async Task T()
        {
            while (Board.Instance == null || Board.Instance.boardEntity.plantArray.Count <= 1) await Task.Delay(1000);
            CoreLogic();
        }

        public static void CoreLogic()
        {
            var plant = Board.Instance.boardEntity.plantArray[0];
            unsafe
            {
                {
                    var a = UnityVersionHandler.Wrap((Il2CppClass*)(Board.Instance.boardEntity.plantArray[0].ObjectClass));
                    var b = UnityVersionHandler.Wrap((Il2CppClass*)(Board.Instance.boardEntity.plantArray[1].ObjectClass));
                    Logger.LogInfo(
                        $"{a == b} {a.Pointer == b.Pointer} {a.ClassPointer == b.ClassPointer} \n" +
                        $"{Board.Instance.boardEntity.plantArray[0].ObjectClass == Board.Instance.boardEntity.plantArray[1].ObjectClass}\n" +
                        $"{(IntPtr)a.ClassPointer:X} {(IntPtr)b.ClassPointer:X} \n" +
                        $"{Board.Instance.boardEntity.plantArray[0].ObjectClass:X} {Board.Instance.boardEntity.plantArray[1].ObjectClass:X} \n" +
                        $"{a.Class == b.Class} {(*a.Class).Equals((*b.Class))}");
                }
                var cls = UnityVersionHandler.Wrap((Il2CppClass*)plant.ObjectClass);
                VirtualInvokeData* vtable = (VirtualInvokeData*)cls.VTable;
                IntPtr methodPtr = vtable[39].methodPtr;

                AttributeEvent = Marshal.GetDelegateForFunctionPointer<AttributeEventDelegate>(methodPtr);
                AttributeEvent.Invoke(plant.Pointer, IntPtr.Zero);

                for (int i = 0; i < cls.VtableCount; ++i)
                {
                    var invoke = ((VirtualInvokeData*)cls.VTable)[i];
                    var m = UnityVersionHandler.Wrap(invoke.method);
                    Logger.LogInfo($"bef i = {i}, {invoke.methodPtr:X} {Marshal.PtrToStringUTF8(m.Name)}");
                }

                try
                {
                    //var vTableAddr = IntPtr.Add(cls.Pointer, GetSize(cls.GetType()));
                    //SystemAPI.VirtualProtectEx(Process.GetCurrentProcess().Handle, vTableAddr, (UIntPtr)(sizeof(VirtualInvokeData*) * cls.VtableCount),
                    //    (uint)SystemAPITools.VirtualProtectExProtection.PAGE_EXECUTE_READWRITE, out var old);

                    //// ((VirtualInvokeData*)cls.VTable)[39].methodPtr = Marshal.GetFunctionPointerForDelegate(CustomAttributeEvent);
                    //// var methodStruct = InternalTools.DetourTools_GetMethod(typeof(Plant), nameof(Plant.AttributeEvent));
                    //// methodStruct.MethodPointer = methodStruct.VirtualMethodPointer = Marshal.GetFunctionPointerForDelegate<AttributeEventDelegate>(CustomAttributeEvent);

                    //var method = UnityVersionHandler.Wrap(((VirtualInvokeData*)vTableAddr)[39].method);
                    //method.MethodPointer = Marshal.GetFunctionPointerForDelegate<AttributeEventDelegate>(CustomAttributeEvent);
                    //((VirtualInvokeData*)vTableAddr)[39].methodPtr = Marshal.GetFunctionPointerForDelegate<AttributeEventDelegate>(CustomAttributeEvent);
                    //((VirtualInvokeData*)vTableAddr)[39].method = method.MethodInfoPointer;

                    //SystemAPI.VirtualProtectEx(Process.GetCurrentProcess().Handle, vTableAddr, (UIntPtr)(sizeof(VirtualInvokeData*) * cls.VtableCount),
                    //    old, out var _);

                    SystemAPI.VirtualProtectEx(Process.GetCurrentProcess().Handle, cls.VTable, (UIntPtr)(sizeof(VirtualInvokeData*) * cls.VtableCount),
                        (uint)SystemAPITools.VirtualProtectExProtection.PAGE_EXECUTE_READWRITE, out var old);

                    ((VirtualInvokeData*)cls.VTable)[39].methodPtr = Marshal.GetFunctionPointerForDelegate<AttributeEventDelegate>(CustomAttributeEvent);
                    var tmp = UnityVersionHandler.Wrap(((VirtualInvokeData*)cls.VTable)[39].method);
                    tmp.MethodPointer = tmp.VirtualMethodPointer = Marshal.GetFunctionPointerForDelegate<AttributeEventDelegate>(CustomAttributeEvent);
                    ((VirtualInvokeData*)cls.VTable)[39].method = tmp.MethodInfoPointer;

                    SystemAPI.VirtualProtectEx(Process.GetCurrentProcess().Handle, cls.VTable, (UIntPtr)(sizeof(VirtualInvokeData*) * cls.VtableCount),
                        old, out var _);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
                Logger.LogInfo(cls.VtableCount);
                for (int i = 0; i < cls.VtableCount; ++i)
                {
                    var invoke = ((VirtualInvokeData*)cls.VTable)[i];
                    var m = UnityVersionHandler.Wrap(invoke.method);
                    Logger.LogInfo($"aft i = {i}, {invoke.methodPtr:X} {m.MethodPointer:X} {Marshal.PtrToStringUTF8(m.Name)}");
                }

                plant.AttributeEvent();
                var e = new TestEvent();
                e.Bind(plant);
                // var d = Marshal.GetDelegateForFunctionPointer<AttributeEventDelegate>(vtable[39].methodPtr);
                // d.Invoke(plant.Pointer, IntPtr.Zero);
            }
        }

        public static int GetSize(Type type)
        {
            // 获取 Unsafe.SizeOf<T> 方法
            var method = typeof(Unsafe).GetMethod(nameof(Unsafe.SizeOf), BindingFlags.Public | BindingFlags.Static);
            // 构造泛型方法 SizeOf<type>
            var genericMethod = method.MakeGenericMethod(type);
            // 调用并返回结果
            return (int)genericMethod.Invoke(null, null);
        }

        readonly void ILoadTask.OnLoad()
        {
            // 安装
            // PlantAttributeEventHook.Install();

            Logger.LogInfo($"{IL2CPP.il2cpp_resolve_icall("UnityEngine.ParticleSystem/MainModule::get_startColor_Injected"):X}");
            Logger.LogInfo($"{IL2CPP.il2cpp_resolve_icall("UnityEngine.ParticleSystem/MainModule::set_startColor_Injected"):X}");
            Logger.LogInfo($"{IL2CPP.RenderTypeName(typeof(ParticleSystem.MainModule))} {IL2CPP.RenderTypeName(typeof(List<int>))}");
            CustomCore.AddTag(PlantType.SunFlower, PlantTags.IsWaterPlant);
            CustomCore.RemoveTag(PlantType.SunFlower, PlantTags.IsWaterPlant);

            var dic = new TypeKeyDictionary<int>();
            dic.Add(5, 114514);
            Logger.LogInfo(dic.GetValue(5));
            Logger.LogInfo(dic.ContainsKey('a'));

            // vtable
            unsafe
            {
                {
                    var strc = UnityVersionHandler.Wrap((Il2CppMethodInfo*)IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Plant>.NativeClassPtr, false, "AttributeEvent", "System.Void"));
                    var ori = strc.MethodPointer;
                    Logger.LogInfo($"{strc.MethodPointer:X} {ori:X}");
                }

                {
                    var strc = UnityVersionHandler.Wrap((Il2CppClass*)IL2CPP.GetIl2CppClass("Assembly-CSharp.dll", "", "Plant"));
                    var ptr = ((VirtualInvokeData*)strc.VTable)[39];
                    Logger.LogInfo($"{ptr.methodPtr:X} {strc}");
                }
            }
            _ = T();
            Logger.LogInfo(GetSize(typeof(NativeClassStructHandler_29_1).Assembly.GetType("Il2CppInterop.Runtime.Runtime.VersionSpecific.Class.NativeClassStructHandler_29_1+Il2CppClass_29_1")));
        }
    }

    public class TestEvent : IPlantEvent
    {
        void IPlantEvent.BeforeSerialized(Trigger trigger, SavePlantData data)
        {
            Logger.LogInfo($"{trigger} {data.thePlantType}");
        }
    }

    //internal struct MyTestDetour : IDetourHook<PlantAwake>
    //{
    //    public MyTestDetour() { }
    //    public int Counter = 0;

    //    public PlantAwake GetHookFunction() => Hook;

    //    public IEnumerable<IntPtr> GetHookTargets() =>
    //        [
    //            InternalTools.DetourTools_GetMethod(typeof(Plant), nameof(Plant.Awake)).MethodPointer,
    //            InternalTools.DetourTools_GetMethod(typeof(Plant), nameof(Plant.Awake)).MethodPointer
    //        ];

    //    public void Hook(IntPtr plant, IntPtr method)
    //    {
    //        IL2CPPDetour
    //        Logger.LogInfo($"log by custom detour hook {this.GetCallOriginalIdx()} {++Counter}");
    //        this.GetOriginals()[0].Invoke(plant, method);
    //    }
    //}

    /// <summary>
    /// 同时 hook Plant.AttributeEvent 的 vtable 槽位和非虚调用入口。
    /// 原生签名: void AttributeEvent(Plant *__this, MethodInfo *method)
    /// </summary>
    //public static unsafe class PlantAttributeEventHook
    //{
    //    // ============================================================
    //    // 0. 日志（写文件，避免 BepInEx Console 吞输出）
    //    // ============================================================
    //    private static readonly string LogPath = Path.Combine(
    //        Path.GetDirectoryName(typeof(PlantAttributeEventHook).Assembly.Location) ?? ".",
    //        "hook_debug.log");

    //    private static readonly object _logLock = new object();
    //    private static bool _verbose = true; // 调试期间打开，稳定后关掉

    //    private static void Log(string msg)
    //    {
    //        if (!_verbose) return;
    //        try
    //        {
    //            lock (_logLock)
    //                File.AppendAllText(LogPath, $"[{DateTime.Now:HH:mm:ss.fff}] {msg}\n");
    //        }
    //        catch { }
    //    }

    //    // ============================================================
    //    // 1. 内存写保护 (Windows)
    //    //    Android/Linux 需要换成 mprotect
    //    // ============================================================
    //    [DllImport("kernel32.dll", SetLastError = true)]
    //    private static extern bool VirtualProtect(
    //        IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

    //    private const uint PAGE_EXECUTE_READWRITE = 0x40;

    //    private static void MakeWritable(IntPtr address, UIntPtr size)
    //    {
    //        if (!VirtualProtect(address, size, PAGE_EXECUTE_READWRITE, out _))
    //            Log($"VirtualProtect 失败, err={Marshal.GetLastWin32Error()}");
    //    }

    //    // ============================================================
    //    // 2. VirtualInvokeData 结构体 (与 il2cpp-class-internals.h 一致)
    //    // ============================================================
    //    [StructLayout(LayoutKind.Sequential)]
    //    private struct VirtualInvokeData
    //    {
    //        public IntPtr methodPtr;   // 实际函数指针
    //        public IntPtr method;      // MethodInfo*
    //    }

    //    // ============================================================
    //    // 3. Hook 委托签名
    //    //    原生: void AttributeEvent(Plant *__this, MethodInfo *method)
    //    // ============================================================
    //    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    //    private delegate void AttributeEventDelegate(IntPtr __this, IntPtr methodInfo);

    //    // ============================================================
    //    // 4. 状态
    //    // ============================================================
    //    private static AttributeEventDelegate _hookDelegate;
    //    private static IntPtr _hookPtr = IntPtr.Zero;

    //    // 每个类单独保存原始 methodPtr（子类 override 后指针可能不同）
    //    private static readonly Dictionary<IntPtr, IntPtr> _originalPtrByClass
    //        = new Dictionary<IntPtr, IntPtr>();
    //    private static readonly Dictionary<IntPtr, string> _classNameByClass
    //        = new Dictionary<IntPtr, string>();

    //    // 目标 slot
    //    private const int SlotIndex = 39;
    //    private const string TargetMethodName = "AttributeEvent";

    //    // 调用计数
    //    private static int _hitCount = 0;

    //    // 监控线程
    //    private static Thread _monitorThread;
    //    private static CancellationTokenSource _monitorCts;

    //    // ============================================================
    //    // 5. 安装
    //    // ============================================================
    //    public static void Install()
    //    {
    //        Log("========== Install 开始 ==========");

    //        IntPtr plantClass = Il2CppClassPointerStore<Plant>.NativeClassPtr;
    //        if (plantClass == IntPtr.Zero) { Log("获取 Plant 类失败"); return; }

    //        string plantName = IL2CPP.il2cpp_class_get_name_(plantClass) ?? "<unknown>";
    //        var nativePlant = UnityVersionHandler.Wrap((Il2CppClass*)plantClass);
    //        Log($"Plant 类: {plantName}, VtableCount={nativePlant.VtableCount}");

    //        // 准备替换委托
    //        _hookDelegate = new AttributeEventDelegate(AttributeEventHook);
    //        _hookPtr = Marshal.GetFunctionPointerForDelegate(_hookDelegate);
    //        Log($"Hook 地址: 0x{_hookPtr:X}");

    //        // 遍历所有子类
    //        HookAllSubclasses(plantClass);

    //        // 启动监控线程（可选）
    //        StartMonitor(5000);

    //        Log("========== Install 完成 ==========");
    //    }

    //    // ============================================================
    //    // 6. 遍历所有 Plant 子类并 hook
    //    // ============================================================
    //    private static void HookAllSubclasses(IntPtr plantClass)
    //    {
    //        Log("--- 遍历所有 Plant 类/子类 ---");

    //        int total = 0;
    //        int matched = 0;

    //        try
    //        {
    //            IntPtr domain = IL2CPP.il2cpp_domain_get();
    //            if (domain == IntPtr.Zero) { Log("domain 为空"); return; }

    //            uint asmCount = 0;
    //            IntPtr* assemblies = IL2CPP.il2cpp_domain_get_assemblies(domain, ref asmCount);
    //            Log($"程序集数量: {asmCount}");

    //            for (uint i = 0; i < asmCount; i++)
    //            {
    //                IntPtr image = IL2CPP.il2cpp_assembly_get_image(assemblies[i]);
    //                if (image == IntPtr.Zero) continue;

    //                uint classCount = IL2CPP.il2cpp_image_get_class_count(image);
    //                for (uint j = 0; j < classCount; j++)
    //                {
    //                    IntPtr klassPtr = IL2CPP.il2cpp_image_get_class(image, j);
    //                    if (klassPtr == IntPtr.Zero) continue;

    //                    // 跳过泛型定义（泛型实例化视情况处理）
    //                    if (IL2CPP.il2cpp_class_is_generic(klassPtr)) continue;

    //                    // 判断是否为 Plant 或 Plant 子类
    //                    bool isPlant = (klassPtr == plantClass);
    //                    bool isSubclass = IL2CPP.il2cpp_class_is_subclass_of(klassPtr, plantClass, false);
    //                    if (!isPlant && !isSubclass) continue;

    //                    total++;

    //                    // 已经 hook 过的跳过（补扫时用）
    //                    if (_originalPtrByClass.ContainsKey(klassPtr)) continue;

    //                    var nativeClass = UnityVersionHandler.Wrap((Il2CppClass*)klassPtr);
    //                    if (nativeClass.VtableCount <= SlotIndex) continue;

    //                    var vtable = (VirtualInvokeData*)nativeClass.VTable;
    //                    if (vtable == null) continue;

    //                    IntPtr miPtr = vtable[SlotIndex].method;
    //                    if (miPtr == IntPtr.Zero) continue;

    //                    // 判断 slot 39 是否是 AttributeEvent
    //                    string methodName = IL2CPP.il2cpp_method_get_name_(miPtr) ?? "";
    //                    if (methodName != TargetMethodName)
    //                    {
    //                        // 名字不匹配，跳过（避免误 hook 其他 slot）
    //                        continue;
    //                    }

    //                    string className = IL2CPP.il2cpp_class_get_name_(klassPtr) ?? "<unknown>";
    //                    string ns = IL2CPP.il2cpp_class_get_namespace_(klassPtr) ?? "";
    //                    string fullName = string.IsNullOrEmpty(ns) ? className : $"{ns}.{className}";

    //                    IntPtr originalPtr = vtable[SlotIndex].methodPtr;

    //                    // 记录原始指针
    //                    _originalPtrByClass[klassPtr] = originalPtr;
    //                    _classNameByClass[klassPtr] = fullName;

    //                    // 修改 vtable
    //                    MakeWritable((IntPtr)(&vtable[SlotIndex].methodPtr), (UIntPtr)IntPtr.Size);
    //                    vtable[SlotIndex].methodPtr = _hookPtr;

    //                    Log($"  ✅ Hook: {fullName} (原始=0x{originalPtr:X})");
    //                    matched++;
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Log($"遍历子类异常: {ex}");
    //        }

    //        Log($"遍历完成: 共 {total} 个 Plant 类/子类, 本次新 hook {matched} 个, 累计 {_originalPtrByClass.Count} 个");
    //        if (_originalPtrByClass.Count == 0)
    //            Log("⚠️ 没有 hook 到任何类，检查 slot 或类名匹配");
    //    }

    //    // ============================================================
    //    // 7. Hook 方法本体
    //    // ============================================================
    //    private static void AttributeEventHook(IntPtr __this, IntPtr methodInfo)
    //    {
    //        int n = Interlocked.Increment(ref _hitCount);

    //        try
    //        {
    //            // 通过 this 找到实际类的 klass
    //            IntPtr klass = IL2CPP.il2cpp_object_get_class(__this);

    //            // 从映射表取该类的原始函数指针
    //            IntPtr originalPtr = IntPtr.Zero;
    //            string className = "<unknown>";
    //            if (klass != IntPtr.Zero)
    //            {
    //                _originalPtrByClass.TryGetValue(klass, out originalPtr);
    //                _classNameByClass.TryGetValue(klass, out className);
    //            }

    //            // ============================================
    //            // 你的业务逻辑
    //            // ============================================
    //            Log($"★ AttributeEvent #{n}: {className} (this=0x{__this:X})");

    //            // 例如访问实例：
    //            // var plant = Il2CppObjectPool.Get<Plant>(__this);
    //            // ...

    //            // ============================================
    //            // 调用原始实现
    //            // ============================================
    //            if (originalPtr != IntPtr.Zero)
    //            {
    //                var original = Marshal.GetDelegateForFunctionPointer<AttributeEventDelegate>(originalPtr);
    //                original(__this, methodInfo);
    //            }
    //            // 空实现共享 ret 的场景不需要调用原始函数，如果没有保存到原始指针也没关系
    //        }
    //        catch (Exception ex)
    //        {
    //            Log($"Hook 异常: {ex}");
    //        }
    //    }

    //    // ============================================================
    //    // 8. 监控线程（检测 vtable 是否被重填 + 补扫新类）
    //    // ============================================================
    //    public static void StartMonitor(int intervalMs = 5000)
    //    {
    //        _monitorCts = new CancellationTokenSource();
    //        var token = _monitorCts.Token;

    //        _monitorThread = new Thread(() =>
    //        {
    //            Log("监控线程启动");
    //            while (!token.IsCancellationRequested)
    //            {
    //                try { MonitorOnce(); }
    //                catch (Exception ex) { Log($"监控异常: {ex.Message}"); }

    //                try { token.WaitHandle.WaitOne(intervalMs); }
    //                catch { break; }
    //            }
    //            Log("监控线程退出");
    //        })
    //        {
    //            IsBackground = true,
    //            Name = "PlantAttributeEventHook.Monitor"
    //        };
    //        _monitorThread.Start();
    //    }

    //    public static void StopMonitor()
    //    {
    //        try
    //        {
    //            _monitorCts?.Cancel();
    //            _monitorThread?.Join(500);
    //        }
    //        catch { }
    //        _monitorCts = null;
    //        _monitorThread = null;
    //    }

    //    private static void MonitorOnce()
    //    {
    //        // (1) 检查已 hook 的类是否被覆盖
    //        int restored = 0;
    //        foreach (var kv in _originalPtrByClass)
    //        {
    //            try
    //            {
    //                IntPtr klassPtr = kv.Key;
    //                var nativeClass = UnityVersionHandler.Wrap((Il2CppClass*)klassPtr);
    //                var vtable = (VirtualInvokeData*)nativeClass.VTable;
    //                if (vtable[SlotIndex].methodPtr != _hookPtr)
    //                {
    //                    MakeWritable((IntPtr)(&vtable[SlotIndex].methodPtr), (UIntPtr)IntPtr.Size);
    //                    vtable[SlotIndex].methodPtr = _hookPtr;
    //                    restored++;
    //                    Log($"⚠️ 重新 hook: {_classNameByClass[klassPtr]}");
    //                }
    //            }
    //            catch { }
    //        }

    //        // (2) 补扫新加载的子类
    //        try
    //        {
    //            IntPtr plantClass = Il2CppClassPointerStore<Plant>.NativeClassPtr;
    //            if (plantClass != IntPtr.Zero)
    //                HookAllSubclasses(plantClass);
    //        }
    //        catch { }
    //    }

    //    // ============================================================
    //    // 9. 卸载
    //    // ============================================================
    //    public static void Uninstall()
    //    {
    //        Log("--- 卸载 ---");
    //        StopMonitor();

    //        foreach (var kv in _originalPtrByClass)
    //        {
    //            try
    //            {
    //                IntPtr klassPtr = kv.Key;
    //                IntPtr originalPtr = kv.Value;
    //                var nativeClass = UnityVersionHandler.Wrap((Il2CppClass*)klassPtr);
    //                var vtable = (VirtualInvokeData*)nativeClass.VTable;
    //                MakeWritable((IntPtr)(&vtable[SlotIndex].methodPtr), (UIntPtr)IntPtr.Size);
    //                vtable[SlotIndex].methodPtr = originalPtr;
    //            }
    //            catch (Exception ex)
    //            {
    //                Log($"恢复失败: {ex.Message}");
    //            }
    //        }
    //        _originalPtrByClass.Clear();
    //        _classNameByClass.Clear();

    //        _hookDelegate = null;
    //        _hookPtr = IntPtr.Zero;
    //        Log("--- 卸载完成 ---");
    //    }

    //    // ============================================================
    //    // 10. 诊断工具：列出当前所有已 hook 的类
    //    // ============================================================
    //    public static void DumpHookedClasses()
    //    {
    //        Log($"当前已 hook {_originalPtrByClass.Count} 个类：");
    //        foreach (var kv in _classNameByClass)
    //            Log($"  {kv.Value} (klass=0x{kv.Key:X})");
    //    }

    //    // ============================================================
    //    // 11. 诊断工具：检查指定类实例的 slot 39
    //    // ============================================================
    //    public static void DiagnoseClass(IntPtr klassPtr)
    //    {
    //        if (klassPtr == IntPtr.Zero) { Log("DiagnoseClass: klassPtr 为空"); return; }

    //        var nativeClass = UnityVersionHandler.Wrap((Il2CppClass*)klassPtr);
    //        string name = IL2CPP.il2cpp_class_get_name_(klassPtr) ?? "<unknown>";

    //        Log($"=== 诊断 {name} ===");
    //        Log($"  VtableCount = {nativeClass.VtableCount}");

    //        if (nativeClass.VtableCount <= SlotIndex)
    //        {
    //            Log($"  vtable 太短，无法访问 slot {SlotIndex}");
    //            return;
    //        }

    //        var vtable = (VirtualInvokeData*)nativeClass.VTable;
    //        IntPtr miPtr = vtable[SlotIndex].method;
    //        IntPtr fnPtr = vtable[SlotIndex].methodPtr;

    //        string miName = miPtr != IntPtr.Zero ? (IL2CPP.il2cpp_method_get_name_(miPtr) ?? "<null>") : "<null>";

    //        Log($"  vtable[{SlotIndex}].methodPtr = 0x{fnPtr:X}");
    //        Log($"  vtable[{SlotIndex}].method    = 0x{miPtr:X} ({miName})");
    //        Log($"  是否已 hook = {_originalPtrByClass.ContainsKey(klassPtr)}");
    //    }
    //}
}
