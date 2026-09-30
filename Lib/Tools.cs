using BepInEx.Unity.IL2CPP;
using BepInEx.Unity.IL2CPP.Hook;
using CustomizeLib.BepInEx.Internal.Tools;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.Runtime.VersionSpecific.MethodInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.LowLevel;

namespace CustomizeLib.BepInEx
{
    // 对外工具类的封装
    public static class Tools
    {
        public const string TypeTools = "TypeTools";
        public const string PlayerLoopTools = "PlayerLoopTools";
        public const string ConsoleTools = "ConsoleTools";
        public const string DetourTools = "DetourTools";

        private static Lazy<Type> InternalToolsType { get; set; } = new(() => typeof(InternalTools));
        private static Dictionary<Tool, MethodInfo> CachedTools { get; set; } = [];

        public static object? CallTool(Tool target, params object[] param)
        {
            if (CachedTools.TryGetValue(target, out var tool))
                return tool.Invoke(null, param); // InternalTools 里的都是 static 方法

            var type = InternalToolsType.Value;
            var genericParamsCount = target.GenericParams != null ? target.GenericParams.Length : 0;
            var method = (MethodInfo)InternalTools.TypeTools_GetMethods(type, $"{target.ToolType}_{target.Name}", genericParamsCount, target.ParamTypes, InternalTools.TypeTools_DefaultFlag).First() ?? 
                throw new ArgumentException($"Couldn't find any matching tool methods");

            if (genericParamsCount > 0)
                method.MakeGenericMethod(target.ParamTypes!);

            CachedTools[target] = method;

            return method.Invoke(null, param);
        }

        public struct Tool(string toolType, string name, Type[]? genericParams = null, Type?[]? paramTypes = null)
        {
            public string ToolType { get; set; } = toolType;
            public string Name { get; set; } = name;
            public Type[]? GenericParams { get; set; } = genericParams;
            public Type?[]? ParamTypes { get; set; } = paramTypes;
        }
    }
}
