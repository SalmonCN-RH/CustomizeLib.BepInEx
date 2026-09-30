using CustomizeLib.BepInEx.Internal.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomizeLib.BepInEx
{
    #region API 用的结构体
    public struct CustomPlant(
        [NotNull] ID id, [NotNull] GameObject prefab, [NotNull] GameObject preview, [NotNull] List<(ID a, ID b)> fusions,
        float attackInterval, float produceInterval, int attackDamage, int maxHealth, float cd, int sun, List<(BulletType bullet, GameObject prefab)> bulletSkins)
    {
        [NotNull] public ID Id { get; set; } = id;
        [NotNull] public GameObject Prefab { get; set; } = prefab;
        [NotNull] public GameObject Preview { get; set; } = preview;
        [NotNull] public List<(ID a, ID b)> Fusions { get; set; } = fusions;
        [NotNull] public List<(BulletType bullet, GameObject prefab)> BulletSkins { get; set; } = bulletSkins;
        public float AttackInterval { get; set; } = attackInterval;
        public float ProduceInterval { get; set; } = produceInterval;
        public int AttackDamage { get; set; } = attackDamage;
        public int MaxHealth { get; set; } = maxHealth;
        public float CD { get; set; } = cd;
        public int Sun { get; set; } = sun;

        internal readonly PlantDataManager.PlantData ToPlantData()
        {
            return new()
            {
                attackDamage = AttackDamage,
                attackInterval = AttackInterval,
                cd = CD,
                cost = Sun,
                maxHealth = MaxHealth,
                produceInterval = ProduceInterval,
                thePlantType = Id
            };
        }
    }

    public readonly struct ID(int id)
    {
        // data
        private readonly int id = id;

        // cons
        public ID(PlantType id) : this((int)id) { }
        public ID(ZombieType id) : this((int)id) { }
        public ID(BulletType id) : this((int)id) { }
        public ID(SoundType id) : this((int)id) { }
        public ID(MusicType id) : this((int)id) { }
        public ID(ParticleType id) : this((int)id) { }

        // ID -> other
        public static implicit operator int(ID id) => id.id;
        public static implicit operator PlantType(ID id) => id.id.ToEnum<PlantType>();
        public static implicit operator ZombieType(ID id) => id.id.ToEnum<ZombieType>();
        public static implicit operator BulletType(ID id) => id.id.ToEnum<BulletType>();
        public static implicit operator SoundType(ID id) => id.id.ToEnum<SoundType>();
        public static implicit operator MusicType(ID id) => id.id.ToEnum<MusicType>();
        public static implicit operator ParticleType(ID id) => id.id.ToEnum<ParticleType>();

        // other -> ID
        public static implicit operator ID(int id) => new(id);
        public static implicit operator ID(PlantType id) => new(id);
        public static implicit operator ID(ZombieType id) => new(id);
        public static implicit operator ID(BulletType id) => new(id);
        public static implicit operator ID(SoundType id) => new(id);
        public static implicit operator ID(MusicType id) => new(id);
        public static implicit operator ID(ParticleType id) => new(id);
    }

    public struct CustomBullet([NotNull] ID id, [NotNull] GameObject prefab)
    {
        [NotNull] public ID Id { get; set; } = id;
        [NotNull] public GameObject Prefab { get; set; } = prefab;
    }

    public struct CustomSound([NotNull] ID id, [NotNull] AudioClip clip)
    {
        [NotNull] public ID Id { get; set; } = id;
        [NotNull] public AudioClip Clip { get; set; } = clip;
    }

    public struct CustomMusic([NotNull] ID id, [NotNull] AudioClip music, [NotNull] string name = "DefaultCustomMusicName")
    {
        [NotNull] public ID Id { get; set; } = id;
        [NotNull] public AudioClip Music { get; set; } = music;
        [NotNull] public string Name { get; set; } = name;
    }

    public struct CustomParticle([NotNull] ID id, [NotNull] GameObject prefab)
    {
        [NotNull] public ID Id { get; set; } = id;
        [NotNull] public GameObject Prefab { get; set; } = prefab;
    }
    #endregion

    #region API 的工具
    public enum PlantTags
    {
        BigNut,
        DoubleBoxPlants,
        FlyingPlants,
        IsCaltrop,
        IsClassicPlant,
        IsFirePlant,
        IsIcePlant,
        IsLily,
        IsMagnetPlants,
        IsNut,
        IsPickaxeStar,
        IsPlantern,
        IsPot,
        IsPotatoMine,
        IsPuff,
        IsPumpkin,
        IsPurplePlant,
        IsSmallRangePlantern,
        IsSnowPlant,
        IsSpickRock,
        IsTallNut,
        IsTangkelp,
        IsTorch,
        IsVirtualPlant,
        IsWaterPlant,
        UmbrellaPlants,
        RedPlant,
        UncrashablePlants,
        WhiteCardPlants
    }

    public enum ZombieTags
    {
        BannedInRandomZombies,
        BigZombie,
        EliteZombie,
        IsAirShipZombie,
        IsAirZombie,
        IsBossZombie,
        IsDriverZombie,
        IsGargantuar,
        IsLeaderZombie,
        NotRandomBungiZombie,
        UltimateZombie,
        UltiZombie_level,
        UltiZombie_level_water,
        UselessHypnoZombie,
        UltiZombie_level_a,
        UltiZombie_level_b,
        UltiZombie_level_c
    }
    #endregion
}
