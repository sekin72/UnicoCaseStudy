using UnicoCaseStudy;
using UnicoCaseStudy.Managers.Pool;
using UnityEngine;

namespace GameClient.GameData
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "UnicoCaseStudy/GameConfig/EnemyConfig", order = 4)]
    public class EnemyConfig : CharacterConfig
    {
        public int Health;
        public float Speed;
    }
}
