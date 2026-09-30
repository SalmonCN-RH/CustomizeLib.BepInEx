using CustomizeLib.BepInEx.Internal.Datas;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomizeLib.BepInEx.Internal.CoreTasks.Register
{
    /// <summary>
    /// 扩容资源数组
    /// </summary>
    internal struct ExpandArray : IGameAppEvent
    {
        readonly int IModTask.GetOrder() => IModTask.Low; // 优先扩容
        readonly IGameAppEvent.GameAppEvent IGameAppEvent.Filter => IGameAppEvent.GameAppEvent.PreLoadResources;

        readonly void IGameAppEvent.OnEvent()
        {
            // 扩容 particlePrefab
            if (RegData.Instance.CustomParticles.Count > 0)
            {
                var max = (long)RegData.Instance.CustomParticles.DefaultIfEmpty().Max(pair => pair.Key) + 1;
                if (max > GameAPP.particlePrefab.Length)
                {
                    var newArr = new Il2CppReferenceArray<GameObject>(max);
                    GameAPP.particlePrefab = newArr;
                }
            }
        }
    }
}
