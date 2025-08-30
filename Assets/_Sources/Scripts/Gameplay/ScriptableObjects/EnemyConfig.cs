using UnicoCaseStudy;
using UnicoCaseStudy.Managers.Pool;
using UnityEditor.Animations;
using UnityEngine;

namespace GameClient.GameData
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "UnicoCaseStudy/GameConfig/EnemyConfig", order = 4)]
    public class EnemyConfig : CharacterConfig
    {
        public AnimatorController AnimatorController;
        public int Health;
        public float Speed;
    }
}
