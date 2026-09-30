using CustomizeLib.BepInEx.Internal.Extensions;
using CustomizeLib.BepInEx.Internal.Tools;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace CustomizeLib.BepInEx.Internal.CoreTasks.Register
{
    /// <summary>
    /// 注入PlayerLoop
    /// </summary>
    internal struct RegLoopSystem : ILoadTask
    {
        readonly void ILoadTask.OnLoad()
        {
            #region 类型注册
            ClassInjector.RegisterTypeInIl2Cpp<Culib_PreUpdate>();
            ClassInjector.RegisterTypeInIl2Cpp<Culib_PostUpdate>();
            ClassInjector.RegisterTypeInIl2Cpp<Culib_PreFixedUpdate>();
            ClassInjector.RegisterTypeInIl2Cpp<Culib_PostFixedUpdate>();
            #endregion

            var root = PlayerLoop.GetCurrentPlayerLoop();

            #region 注入 Update
            root = InternalTools.PlayerLoopTools_InsertSystem(
                root: root,
                target: (loop) => InternalTools.PlayerLoopTools_CheckByType(loop, typeof(Update.ScriptRunBehaviourUpdate)), // 在 Update 执行之前
                cons: () => InternalTools.PlayerLoopTools_ConsLoopSystem(typeof(Culib_PreUpdate), () =>
                {
                    // Pre OnUpdate
                    IPlantEvent.EventMaps[IPlantEvent.EventType.OnUpdate].Invoke(Trigger.Pre, []);
                    IPlantEvent.EventMaps[IPlantEvent.EventType.OnUpdateMustExecute].Invoke(Trigger.Pre, []);
                }));

            root = InternalTools.PlayerLoopTools_AppendSystem(
                root: root,
                target: (loop) => InternalTools.PlayerLoopTools_CheckByType(loop, typeof(Update.ScriptRunBehaviourUpdate)), // 在 Update 执行之后
                cons: () => InternalTools.PlayerLoopTools_ConsLoopSystem(typeof(Culib_PostUpdate), () =>
                {
                    // Post OnUpdate
                    IPlantEvent.EventMaps[IPlantEvent.EventType.OnUpdate].Invoke(Trigger.Post, []);
                    IPlantEvent.EventMaps[IPlantEvent.EventType.OnUpdateMustExecute].Invoke(Trigger.Post, []);
                }));
            #endregion

            #region 注入 FixedUpdate
            root = InternalTools.PlayerLoopTools_InsertSystem(
                root: root,
                target: (loop) => InternalTools.PlayerLoopTools_CheckByType(loop, typeof(FixedUpdate.ScriptRunBehaviourFixedUpdate)), // 在 FixedUpdate 执行之前
                cons: () => InternalTools.PlayerLoopTools_ConsLoopSystem(typeof(Culib_PreFixedUpdate), () =>
                {
                    // Pre OnFixedUpdate
                    IPlantEvent.EventMaps[IPlantEvent.EventType.OnFixedUpdate].Invoke(Trigger.Pre, []);
                    IPlantEvent.EventMaps[IPlantEvent.EventType.OnFixedUpdateMustExecute].Invoke(Trigger.Pre, []);
                }));

            root = InternalTools.PlayerLoopTools_AppendSystem(
                root: root,
                target: (loop) => InternalTools.PlayerLoopTools_CheckByType(loop, typeof(FixedUpdate.ScriptRunBehaviourFixedUpdate)), // 在 FixedUpdate 执行之后
                cons: () => InternalTools.PlayerLoopTools_ConsLoopSystem(typeof(Culib_PostFixedUpdate), () =>
                {
                    // Post OnFixedUpdate
                    IPlantEvent.EventMaps[IPlantEvent.EventType.OnFixedUpdate].Invoke(Trigger.Post, []);
                    IPlantEvent.EventMaps[IPlantEvent.EventType.OnFixedUpdateMustExecute].Invoke(Trigger.Post, []);
                }));
            #endregion

            PlayerLoop.SetPlayerLoop(root);
        }

        #region 注入的 PlayerLoop 绑定的类
        private class Culib_PreUpdate : Il2CppSystem.Object { }
        private class Culib_PostUpdate : Il2CppSystem.Object { }
        private class Culib_PreFixedUpdate : Il2CppSystem.Object { }
        private class Culib_PostFixedUpdate : Il2CppSystem.Object { }
        #endregion
    }
}
