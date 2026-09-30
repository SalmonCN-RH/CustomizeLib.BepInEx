using CustomizeLib.BepInEx.Internal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomizeLib.BepInEx.Internal.Datas
{
    // 存储注册进来的数据
    internal struct RegData
    {
        #region 基础成员
        internal static RegData Instance;

        public RegData()
        {
            Instance = this;
            Initialize();
        }
        #endregion

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void Initialize()
        {
            TypeExtraMgr = new();
        }

        #region TypeMgrExtra
        /// <inheritdoc cref="TypeExtra.AddTag(PlantType, string)"/>
        internal readonly void AddTag(PlantType plantType, string tag) =>
            TypeExtraMgr.AddTag(plantType, tag);

        /// <inheritdoc cref="TypeExtra.RemoveTag(PlantType, string)"/>
        internal readonly void RemoveTag(PlantType plantType, string tag) =>
            TypeExtraMgr.RemoveTag(plantType, tag);

        /// <inheritdoc cref="TypeExtra.AddTag(ZombieType, string)"/>
        internal readonly void AddTag(ZombieType zombieType, string tag) =>
            TypeExtraMgr.AddTag(zombieType, tag);
        /// <inheritdoc cref="TypeExtra.RemoveTag(ZombieType, string)"/>
        internal readonly void RemoveTag(ZombieType zombieType, string tag) =>
            TypeExtraMgr.RemoveTag(zombieType, tag);
        #endregion

        /// <summary>
        /// 注册二创植物
        /// </summary>
        /// <typeparam name="TBase">植物基类</typeparam>
        /// <typeparam name="TBehaviour">自定义植物类</typeparam>
        /// <param name="customPlant">植物数据</param>
        readonly internal void RegisterCustomPlant<TBase, TBehaviour>(CustomPlant customPlant) where TBase : Plant where TBehaviour : MonoBehaviour
        {
            customPlant.Prefab.AddComponent<TBase>().thePlantType = customPlant.Id;
            customPlant.Prefab.AddComponent<TBehaviour>();

            foreach (var (a, b) in customPlant.Fusions)
                AddFusion(a, b, customPlant.Id);

            CustomPlants.AddAndLogIfDup(customPlant.Id, customPlant, $"Duplicate plant id {(int)customPlant.Id}");
        }

        /// <summary>
        /// 注册二创植物
        /// </summary>
        /// <typeparam name="TBase">植物基类</typeparam>
        /// <param name="customPlant">植物数据</param>
        readonly internal void RegisterCustomPlant<TBase>(CustomPlant customPlant) where TBase : Plant
        {
            customPlant.Prefab.AddComponent<TBase>().thePlantType = customPlant.Id;

            foreach (var (a, b) in customPlant.Fusions)
                AddFusion(a, b, customPlant.Id);

            CustomPlants.AddAndLogIfDup(customPlant.Id, customPlant, $"Duplicate plant id {(int)customPlant.Id}");
        }

        /// <summary>
        /// 注册融合配方
        /// </summary>
        /// <param name="a">底植物</param>
        /// <param name="b">融合上去的植物</param>
        /// <param name="res">融合结果</param>
        readonly internal void AddFusion(ID a, ID b, ID res) => CustomFusions.Add((a, b, res));

        /// <summary>
        /// 注册二创子弹
        /// </summary>
        /// <typeparam name="TBase">子弹基类</typeparam>
        /// <typeparam name="TBehaviour">自定义子弹类</typeparam>
        /// <param name="customBullet">子弹数据</param>
        readonly internal void RegisterCustomBullet<TBase, TBehaviour>(CustomBullet customBullet) where TBase : Bullet where TBehaviour : MonoBehaviour
        {
            customBullet.Prefab.AddComponent<TBase>().theBulletType = customBullet.Id;
            customBullet.Prefab.AddComponent<TBehaviour>();

            CustomBullets.AddAndLogIfDup(customBullet.Id, customBullet, $"Duplicate bullet id {(int)customBullet.Id}");
        }

        /// <summary>
        /// 注册二创子弹
        /// </summary>
        /// <typeparam name="TBase">子弹基类</typeparam>
        /// <param name="customBullet">子弹数据</param>
        readonly internal void RegisterCustomBullet<TBase>(CustomBullet customBullet) where TBase : Bullet
        {
            customBullet.Prefab.AddComponent<TBase>().theBulletType = customBullet.Id;

            CustomBullets.AddAndLogIfDup(customBullet.Id, customBullet, $"Duplicate bullet id {(int)customBullet.Id}");
        }

        /// <summary>
        /// 注册二创音效
        /// </summary>
        /// <param name="customSound">音效数据</param>
        readonly internal void RegisterCustomSound(CustomSound customSound)
        {
            CustomSounds.AddAndLogIfDup(customSound.Id, customSound, $"Duplicate sound id {(int)customSound.Id}");
        }

        /// <summary>
        /// 注册二创音乐
        /// </summary>
        /// <param name="customMusic">音乐数据</param>
        readonly internal void RegisterCustomMusic(CustomMusic customMusic)
        {
            CustomMusics.AddAndLogIfDup(customMusic.Id, customMusic, $"Duplicate music id {(int)customMusic.Id}");
        }

        /// <summary>
        /// 注册二创粒子
        /// </summary>
        /// <param name="customParticle">粒子数据</param>
        readonly internal void RegisterCustomParticle(CustomParticle customParticle)
        {
            CustomParticles.AddAndLogIfDup(customParticle.Id, customParticle, $"Duplicate particle id {(int)customParticle.Id}");
        }

        #region 数据
        /// <summary>
        /// 植物类型数据
        /// </summary>
        internal TypeExtra TypeExtraMgr { get; set; } = new();

        /// <summary>
        /// 二创植物
        /// </summary>
        internal Dictionary<PlantType, CustomPlant> CustomPlants { get; set; } = [];

        /// <summary>
        /// <para>融合配方</para>
        /// <para>a: 底植物 b: 融合上去的植物 c: 融合结果</para>
        /// </summary>
        internal List<(ID a, ID b, ID res)> CustomFusions { get; set; } = [];

        /// <summary>
        /// 二创子弹
        /// </summary>
        internal Dictionary<BulletType, CustomBullet> CustomBullets { get; set; } = [];

        /// <summary>
        /// 二创音效
        /// </summary>
        internal Dictionary<SoundType, CustomSound> CustomSounds { get; set; } = [];

        /// <summary>
        /// 二创音乐
        /// </summary>
        internal Dictionary<MusicType, CustomMusic> CustomMusics { get; set; } = [];

        /// <summary>
        /// 二创粒子
        /// </summary>
        internal Dictionary<ParticleType, CustomParticle> CustomParticles { get; set; } = [];
        #endregion
    }
}
