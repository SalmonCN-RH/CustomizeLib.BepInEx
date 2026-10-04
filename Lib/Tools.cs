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
        private static Dictionary<Tool, FieldInfo> CachedFields { get; set; } = [];
        private static Dictionary<Tool, PropertyInfo> CachedProps { get; set; } = [];

        #region 方法
        /// <summary>
        /// 调用 <see cref="InternalTools"/> 的工具方法
        /// </summary>
        /// <param name="target">目标方法</param>
        /// <param name="param">参数</param>
        /// <returns>方法返回值</returns>
        /// <exception cref="ArgumentException">找不到目标方法时抛出</exception>
        public static object? CallToolMethod(Tool target, params object[] param)
        {
            if (CachedTools.TryGetValue(target, out var m)) // m = method
                return m.Invoke(null, param); // InternalTools 里的都是 static 方法

            var type = InternalToolsType.Value;
            var genericParamsCount = target.GenericParams != null ? target.GenericParams.Length : 0;
            var method = (MethodInfo)InternalTools.TypeTools_GetMethods(type, target.GetTargetName(), genericParamsCount, target.ParamTypes, InternalTools.TypeTools_DefaultFlag).First() ?? 
                throw new ArgumentException($"Couldn't find any matching tool methods");

            if (genericParamsCount > 0)
                method.MakeGenericMethod(target.ParamTypes!);

            CachedTools[target] = method;

            return method.Invoke(null, param);
        }
        #endregion

        #region 字段
        /// <summary>
        /// 初始化目标字段
        /// </summary>
        /// <param name="target">目标字段</param>
        /// <returns>字段的 <see cref="FieldInfo"/></returns>
        /// <exception cref="ArgumentException">找不到字段时抛出</exception>
        private static FieldInfo InitToolField(Tool target)
        {
            if (CachedFields.TryGetValue(target, out var f)) // f = field
                return f;

            var type = InternalToolsType.Value;
            var field = type.GetField(target.GetTargetName(), InternalTools.TypeTools_DefaultFlag) ??
                throw new ArgumentException($"Couldn't find any matching tool fields");

            CachedFields[target] = field;
            return field;
        }

        /// <summary>
        /// 获取 <see cref="InternalTools"/> 的工具字段
        /// </summary>
        /// <param name="target">目标字段</param>
        /// <returns>字段值</returns>
        public static object? GetToolField(Tool target) => InitToolField(target).GetValue(null);

        /// <summary>
        /// 设置 <see cref="InternalTools"/> 的工具字段
        /// </summary>
        /// <param name="target">目标字段</param>
        /// <param name="value">设置值</param>
        /// <returns>字段值</returns>
        public static object? SetToolField(Tool target, object? value)
        {
            InitToolField(target).SetValue(null, value);
            return GetToolField(target);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 初始化目标属性
        /// </summary>
        /// <param name="target">目标属性</param>
        /// <returns>字段的 <see cref="PropertyInfo"/></returns>
        /// <exception cref="ArgumentException">找不到属性时抛出</exception>
        private static PropertyInfo InitToolProp(Tool target)
        {
            if (CachedProps.TryGetValue(target, out var p)) // p = property
                return p;

            var type = InternalToolsType.Value;
            var prop = type.GetProperty(target.GetTargetName(), InternalTools.TypeTools_DefaultFlag) ??
                throw new ArgumentException($"Couldn't find any matching tool porperties");

            CachedProps[target] = prop;
            return prop;
        }

        /// <summary>
        /// 获取 <see cref="InternalTools"/> 的工具属性
        /// </summary>
        /// <param name="target">目标属性</param>
        /// <returns>属性值</returns>
        public static object? GetToolProp(Tool target) => InitToolProp(target).GetValue(null);

        /// <summary>
        /// 设置 <see cref="InternalTools"/> 的工具属性
        /// </summary>
        /// <param name="target">目标属性</param>
        /// <param name="value">设置值</param>
        /// <returns>属性值</returns>
        public static object? SetToolProp(Tool target)
        {
            InitToolProp(target).SetValue(null, null);
            return GetToolProp(target);
        }
        #endregion

        public struct Tool(string toolType, string name, Type[]? genericParams = null, Type?[]? paramTypes = null)
        {
            internal readonly string GetTargetName() => $"{ToolType}_{Name}";

            public string ToolType { get; set; } = toolType;
            public string Name { get; set; } = name;
            public Type[]? GenericParams { get; set; } = genericParams;
            public Type?[]? ParamTypes { get; set; } = paramTypes;
        }
    }
}
