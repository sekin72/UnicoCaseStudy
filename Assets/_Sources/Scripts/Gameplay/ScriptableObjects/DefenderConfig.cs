using UnicoCaseStudy;
using UnicoCaseStudy.Managers.Pool;
using UnityEngine;

namespace GameClient.GameData
{
    [CreateAssetMenu(fileName = "DefenderConfig", menuName = "UnicoCaseStudy/GameConfig/DefenderConfig", order = 3)]
    public class DefenderConfig : CharacterConfig
    {
        public PoolKeys IdleVFXPoolKey;

        public float PlacementCooldown = 0;
        public int Damage;
        public int Range = 1;
        public float AttackCooldown;
        public Vector3Int Direction = Vector3Int.forward;
    }
}
