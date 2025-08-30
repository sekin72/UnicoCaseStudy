using System;
using UnicoCaseStudy;
using UnityEngine;

namespace GameClient.GameData
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "UnicoCaseStudy/GameConfig/EnemyConfig", order = 4)]
    public class EnemyConfig : CharacterConfig
    {
        public RuntimeAnimatorController AnimatorController;
        public int Health;
        public float Speed;
    }
}
