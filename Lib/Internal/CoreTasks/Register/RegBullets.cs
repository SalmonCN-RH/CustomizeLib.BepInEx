using CustomizeLib.BepInEx.Internal.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.CoreTasks.Register
{
    /// <summary>
    /// 注册二创子弹
    /// </summary>
    internal struct RegBullets : IGameAppEvent
    {
        readonly IGameAppEvent.GameAppEvent IGameAppEvent.Filter => IGameAppEvent.GameAppEvent.PreLoadResources;

        readonly void IGameAppEvent.OnEvent()
        {
            foreach (var (bt, data) in RegData.Instance.CustomBullets)
            {
                if (GameAPP.resourcesManager.allBullets.Contains(bt)) continue;

                GameAPP.resourcesManager.bulletPrefabs[bt] = data.Prefab;
                GameAPP.resourcesManager.allBullets.Add(bt);
            }
        }
    }
}
