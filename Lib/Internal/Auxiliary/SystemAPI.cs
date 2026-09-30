using CustomizeLib.BepInEx.Internal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomizeLib.BepInEx.Internal.Auxiliary
{
    internal static class SystemAPI
    {
        private delegate bool VirtualProtectExDelegate(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        #pragma warning disable IDE1006

        private static VirtualProtectExDelegate? d_VirtualProtectEx { get; set; }

        #pragma warning restore IDE1006

        static SystemAPI()
        {
            // VirtualProtectEx
            if (!IsAPIAvailable(SystemAPITools.APINames.VirtualProtectEx))
                if (TryLoad<VirtualProtectExDelegate>(SystemAPITools.DllNames.Kernel32, SystemAPITools.APINames.VirtualProtectEx, out var virtualProtectEx,
                    RuntimePlatform.WindowsPlayer, RuntimePlatform.WindowsEditor, RuntimePlatform.WindowsServer))
                    d_VirtualProtectEx = virtualProtectEx;
        }

        #region 工具方法
        /// <summary>
        /// <paramref name="apiName"/> 是否可用
        /// </summary>
        /// <param name="apiName">API 名</param>
        /// <returns>是否可用</returns>
        /// <exception cref="ArgumentException"></exception>
        internal static bool IsAPIAvailable(string apiName)
        {
            var prop = typeof(SystemAPI).GetProperty($"d_{apiName}", InternalTools.TypeTools_DefaultFlag) ?? throw new ArgumentException($"Not found api name {apiName} in SystemAPI");
            if (prop.GetValue(null) == null) return false;
            return true;
        }

        /// <summary>
        /// 尝试从 <paramref name="dll"/> 加载导出函数名为 <paramref name="name"/> 的函数
        /// </summary>
        /// <typeparam name="TDelegate">函数委托</typeparam>
        /// <param name="dll">库名</param>
        /// <param name="name">函数名</param>
        /// <param name="d">获取到的导出函数</param>
        /// <param name="platforms">可用平台</param>
        /// <returns>是否成功加载</returns>
        internal static bool TryLoad<TDelegate>(string dll, string name, out TDelegate d, params RuntimePlatform[] platforms) where TDelegate : Delegate
        {
            d = default!;
            if (platforms.Length > 0 && !platforms.Contains(Application.platform)) return false;
            try
            {
                var handle = NativeLibrary.Load(dll);
                if (handle == IntPtr.Zero) return false;
                var func = NativeLibrary.GetExport(handle, name);
                if (func == IntPtr.Zero)
                {
                    NativeLibrary.Free(handle);
                    return false;
                }

                d = Marshal.GetDelegateForFunctionPointer<TDelegate>(func);
                return true;
            }
            catch (DllNotFoundException) { }
            catch (Exception ex)
            {
                Logger.LogError(
                        $"Error in load system api: \n" +
                        $"Message: {ex.Message}\n" +
                        $"{ex.StackTrace}\n" +
                        $"ToString: \n" +
                        $"{ex}");
            }
            return false;
        }
        #endregion

        internal static bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect) =>
            d_VirtualProtectEx!.Invoke(hProcess, lpAddress, dwSize, flNewProtect, out lpflOldProtect);
    }
    
    internal static class SystemAPITools
    {
        internal static class APINames
        {
            internal const string VirtualProtectEx = "VirtualProtectEx";
            internal const string VirtualProtect = "VirtualProtect";
        }

        internal static class DllNames
        {
            internal const string Kernel32 = "kernel32.dll";
        }

        /// <summary>
        /// Win32 Memory Protection Constants
        /// </summary>
        [Flags]
        internal enum VirtualProtectExProtection : uint
        {
            /// <summary>
            /// 0x01 禁止所有访问
            /// </summary>
            PAGE_NOACCESS = 0x01,
            /// <summary>
            /// 0x02 只读访问
            /// </summary>
            PAGE_READONLY = 0x02,
            /// <summary>
            /// 0x04 读写访问
            /// </summary>
            PAGE_READWRITE = 0x04,
            /// <summary>
            /// 0x08 写时复制访问
            /// </summary>
            PAGE_WRITECOPY = 0x08,
            /// <summary>
            /// 0x10 仅执行访问
            /// </summary>
            PAGE_EXECUTE = 0x10,
            /// <summary>
            /// 0x20 执行 + 只读访问
            /// </summary>
            PAGE_EXECUTE_READ = 0x20,
            /// <summary>
            /// 0x40 执行 + 读写访问
            /// </summary>
            PAGE_EXECUTE_READWRITE = 0x40,
            /// <summary>
            /// 0x80 执行 + 写时复制访问
            /// </summary>
            PAGE_EXECUTE_WRITECOPY = 0x80,
            /// <summary>
            /// 0x100 保护页, 首次访问会触发 STATUS_GUARD_PAGE 异常
            /// </summary>
            PAGE_GUARD = 0x100,
            /// <summary>
            /// 0x200 禁止缓存
            /// </summary>
            PAGE_NOCACHE = 0x200,
            /// <summary>
            /// 0x400 写合并
            /// </summary>
            PAGE_WRITECOMBINE = 0x400
        }
    }
}
