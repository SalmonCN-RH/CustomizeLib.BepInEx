using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class CuLibLoad : Attribute { }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class RegisterTypeInIl2Cpp(bool logSuccess = true, bool disableErrorMessage = false) : Attribute
    {
        public bool LogSuccess { get; set; } = logSuccess;
        public bool DisableErrorMessage { get; set; } = disableErrorMessage;

        internal static void ProcessAll()
        {
            foreach (var asm in Utils.TypeUtils.GetAllAssemblies())
            {
                foreach (var type in Utils.TypeUtils.GetAllTypes(asm))
                {
                    var arr = type.GetCustomAttributes(typeof(RegisterTypeInIl2Cpp), inherit: true);
                    if (arr == null || arr.Length <= 0) continue;
                    var attr = (RegisterTypeInIl2Cpp)arr[0];
                    if (attr == null) continue;

                    bool flag = false;
                    if (attr.DisableErrorMessage)
                        Utils.LoggerUtils.AddOnLog(args =>
                        {
                            var type = args[3]!.GetType();
                            if ((MSLogLevel)args[1]! == MSLogLevel.Error &&  // 1 是 logLevel
                                type == AccessTools.TypeByName("Microsoft.Extensions.Logging.FormattedLogValues"))
                            {
                                if (type.GetField("_originalMessage", Utils.TypeUtils.DefaultDeclaredOnly)!.GetValue(args[3])!.ToString() ==
                                    "Assembly {AssemblyName} is not registered in il2cpp")
                                {
                                    Utils.LoggerUtils.RemoveLevel(MSLogLevel.Error);
                                    flag = true;
                                }
                            }
                        });

                    ClassInjector.RegisterTypeInIl2Cpp(type, new()
                    {
                        LogSuccess = attr.LogSuccess
                    });

                    if (attr.DisableErrorMessage && flag)
                        Utils.LoggerUtils.AddLevel(MSLogLevel.Error);
                }
            }
        }
    }
}
