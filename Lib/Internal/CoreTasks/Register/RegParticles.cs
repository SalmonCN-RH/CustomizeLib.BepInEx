using CustomizeLib.BepInEx.Internal.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.CoreTasks.Register
{
    /// <summary>
    /// 注册二创粒子
    /// </summary>
    internal struct RegParticles : IGameAppEvent
    {
        readonly IGameAppEvent.GameAppEvent IGameAppEvent.Filter => IGameAppEvent.GameAppEvent.PreLoadResources;

        readonly void IGameAppEvent.OnEvent()
        {
            foreach (var (pt, data) in RegData.Instance.CustomParticles)
            {
                if (GameAPP.resourcesManager.allParticles.Contains(pt)) continue;

                GameAPP.particlePrefab[(int)pt] = data.Prefab;
                GameAPP.resourcesManager.particlePrefabs[pt] = data.Prefab;
                GameAPP.resourcesManager.allParticles.Add(pt);
            }
        }
    }
}
