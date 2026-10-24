using HarmonyLib;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx
{
    public static partial class Utils
    {
        public static class LoggerUtils
        {
            /// <inheritdoc cref="LoggerWrapper.AddLevel(LogLevel)"/>
            public static void AddLevel(MSLogLevel level) => LoggerWrapper.AddLevel(level);
            /// <inheritdoc cref="LoggerWrapper.RemoveLevel(LogLevel)"/>
            public static void RemoveLevel(MSLogLevel level) => LoggerWrapper.RemoveLevel(level);
            /// <inheritdoc cref="LoggerWrapper.AddOnBeginScope(Action{object?[]})"/>
            public static void AddOnBeginScope(Action<object?[]> callback) => LoggerWrapper.AddOnBeginScope(callback);
            /// <inheritdoc cref="LoggerWrapper.AddOnIsEnabled(Action{object?[]})"/>
            public static void AddOnIsEnabled(Action<object?[]> callback) => LoggerWrapper.AddOnIsEnabled(callback);
            /// <inheritdoc cref="LoggerWrapper.AddOnLog(Action{object?[]})"/>
            public static void AddOnLog(Action<object?[]> callback) => LoggerWrapper.AddOnLog(callback);
            /// <inheritdoc cref="LoggerWrapper.DisableLog(int)"/>
            public static void DisableLog(int count = 1) => LoggerWrapper.DisableLog(count);
            /// <inheritdoc cref="LoggerWrapper.DisableLog(LogLevel, int)"/>
            public static void DisableLog(MSLogLevel level, int count = 1) => LoggerWrapper.DisableLog(level, count);
            /// <inheritdoc cref="LoggerWrapper.GetLevels"/>
            public static HashSet<MSLogLevel> GetLevels() => LoggerWrapper.GetLevels();
            /// <inheritdoc cref="LoggerWrapper.SetLevels(HashSet{LogLevel})"/>
            public static void SetLevels(HashSet<MSLogLevel> levels) => LoggerWrapper.SetLevels(levels);

            public class LoggerWrapper : ILogger
            {
                /// <summary>
                /// 添加允许显示的日志等级
                /// </summary>
                /// <param name="level">日志等级</param>
                public static void AddLevel(MSLogLevel level) => Levels.Add(level);
                /// <summary>
                /// 删除允许显示的日志等级
                /// </summary>
                /// <param name="level">日志等级</param>
                public static void RemoveLevel(MSLogLevel level) => Levels.Remove(level);
                /// <summary>
                /// 添加调用 BeginScope 时调用的回调
                /// </summary>
                /// <param name="callback">回调</param>
                public static void AddOnBeginScope(Action<object?[]> callback) => OnBeginScope.Add(callback);
                /// <summary>
                /// 添加调用 IsEnabled 时调用的回调
                /// </summary>
                /// <param name="callback">回调</param>
                public static void AddOnIsEnabled(Action<object?[]> callback) => OnIsEnabled.Add(callback);
                /// <summary>
                /// 添加调用 Log 时调用的回调
                /// </summary>
                /// <param name="callback">回调</param>
                public static void AddOnLog(Action<object?[]> callback) => OnLog.Add(callback);
                /// <summary>
                /// 禁用此后 <paramref name="count"/> 次输出
                /// </summary>
                /// <param name="count">次数</param>
                public static void DisableLog(int count = 1) => AddOnLog(new DisableLog_Default_Closure(count).OnLog);
                /// <summary>
                /// 禁用此后 <paramref name="count"/> 次日志等级为 <paramref name="level"/> 的输出
                /// </summary>
                /// <param name="level">日志等级</param>
                /// <param name="count">次数</param>
                public static void DisableLog(MSLogLevel level, int count = 1) => AddOnLog(new DisableLog_Level_Closure(level, count).OnLog);
                /// <summary>
                /// 获取当前允许输出的日志等级
                /// </summary>
                /// <returns>当前允许输出的日志等级</returns>
                public static HashSet<MSLogLevel> GetLevels() => Levels;
                /// <summary>
                /// 设置当前允许输出的日志等级
                /// </summary>
                /// <param name="levels">当前允许输出的日志等级</param>
                public static void SetLevels(HashSet<MSLogLevel> levels) => Levels = levels;

                private static LoggerWrapper? Instance;
                private static HashSet<MSLogLevel> Levels { get; set; } = [];
                // 事件 callback
                private static List<Action<object?[]>> OnBeginScope { get; set; } = [];
                private static List<Action<object?[]>> OnIsEnabled { get; set; } = [];
                private static List<Action<object?[]>> OnLog { get; set; } = [];

                private ILogger Logger { get; set; }

                internal LoggerWrapper()
                {
                    Instance = this;
                    var interopLogger = AccessTools.TypeByName("Il2CppInterop.Common.Logger");
                    var instance = interopLogger.GetProperty("Instance", TypeUtils.DefaultDeclaredOnly)!;

                    Logger = (ILogger)instance.GetValue(null)!;

                    foreach (var level in Enum.GetValues<MSLogLevel>())
                        if (Logger.IsEnabled(level)) AddLevel(level);

                    instance.SetValue(null, this);
                }

                public IDisposable BeginScope<TState>(TState state)
                {
                    var original = OnBeginScope.ToArray();
                    OnBeginScope.Clear();
                    original.DoAll(action => action.Invoke([typeof(TState), state]));

                    return Logger.BeginScope(state);
                }

                public bool IsEnabled(MSLogLevel logLevel)
                {
                    var original = OnIsEnabled.ToArray();
                    OnIsEnabled.Clear();
                    original.DoAll(action => action.Invoke([logLevel]));

                    return Logger.IsEnabled(logLevel) && Levels.Contains(logLevel);
                }

                public void Log<TState>(MSLogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
                {
                    var original = OnLog.ToArray();
                    OnLog.Clear();
                    original.DoAll(action => action.Invoke([typeof(TState), logLevel, eventId, state, exception, formatter]));

                    if (!Levels.Contains(logLevel)) return;
                    Logger.Log(logLevel, eventId, state, exception, formatter);
                }

                private class DisableLog_Default_Closure
                {
                    internal DisableLog_Default_Closure(int count = 1)
                    {
                        _levels = [.. Levels];
                        _count = count;
                    }

                    private readonly MSLogLevel[] _levels = [];
                    private int _count = 0;

                    internal void OnLog(object?[] args)
                    {
                        foreach (var level in Levels)
                            RemoveLevel(level);
                        _count--;
                        if (_count >= 0)
                            AddOnLog(OnLog);
                        else
                            foreach (var level in _levels)
                                AddLevel(level);
                    }
                }

                private class DisableLog_Level_Closure(MSLogLevel target, int count = 1)
                {
                    private int _count = count;

                    internal void OnLog(object?[] args)
                    {
                        if ((MSLogLevel)args[1]! == target) // 参数 1 是 logLevel
                        {
                            RemoveLevel(target);
                            _count--;
                            if (_count >= 0) AddOnLog(OnLog);
                            else AddLevel(target);
                        }
                        else AddOnLog(OnLog);
                    }
                }
            }
        }
    }
}
