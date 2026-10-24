using CustomizeLib.BepInEx.Internal.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomizeLib.BepInEx.Internal.Mod
{
    internal static class ModImpl
    {
        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="audio">音效</param>
        /// <param name="volume">音量</param>
        public static void PlaySound(AudioClip audio, float volume = 1.0f)
        {
            GameAPP.music.PlayOneShot(audio, volume * GameAPP.config.gameSoundVolume);
        }
    }
}
