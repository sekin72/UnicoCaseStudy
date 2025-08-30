using System;
using UnicoCaseStudy;
using UnicoCaseStudy.Managers.Pool;
using UnityEngine;

namespace GameClient.GameData
{
    [CreateAssetMenu(fileName = "DefenderConfig", menuName = "UnicoCaseStudy/GameConfig/DefenderConfig", order = 3)]
    public class DefenderConfig : CharacterConfig
    {
        public PoolKeys IdleVFXPoolKey;
        public PoolKeys BulletPoolKey;
        public PoolKeys ImpactPoolKey;

        public float PlacementCooldown = 0;
        public int Damage;
        public int Range = 1;
        public float AttackCooldown;
        public AttackDirection Direction = AttackDirection.Forward;
    }

    public enum AttackDirection
    {
        Forward,
        All
    }
}
