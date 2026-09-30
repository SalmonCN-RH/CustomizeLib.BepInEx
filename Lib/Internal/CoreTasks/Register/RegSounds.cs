using CustomizeLib.BepInEx.Internal.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.CoreTasks.Register
{
    /// <summary>
    /// 注册二创音效
    /// </summary>
    internal struct RegSounds : IGameAppEvent
    {
        readonly IGameAppEvent.GameAppEvent IGameAppEvent.Filter => IGameAppEvent.GameAppEvent.PostLoadResources;

        readonly void IGameAppEvent.OnEvent()
        {
            foreach (var (st, data) in RegData.Instance.CustomSounds)
            {
                if (GameAPP.soundManager.sounds.ContainsKey(st)) continue;

                GameAPP.soundManager.sounds.Add(st, data.Clip);
            }
        }
    }
}
