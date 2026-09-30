using CustomizeLib.BepInEx.Internal.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.CoreTasks.Register
{
    /// <summary>
    /// 注册二创音乐
    /// </summary>
    internal struct RegMusics : IGameAppEvent
    {
        readonly IGameAppEvent.GameAppEvent IGameAppEvent.Filter => IGameAppEvent.GameAppEvent.PostLoadResources;

        readonly void IGameAppEvent.OnEvent()
        {
            foreach (var (mt, data) in RegData.Instance.CustomMusics)
            {
                if (GameAPP.soundManager.musics.ContainsKey(mt)) continue;

                GameAPP.soundManager.musics.Add(mt, data.Music);
                SoundManager.MusicNames.Add(mt, data.Name);
            }
        }
    }
}
