using System.Collections.Generic;
using GameClient.GameData;
using UnityEngine;

namespace UnicoCaseStudy
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "UnicoCaseStudy/GameConfig/LevelConfig", order = 1)]
    public class LevelConfig : ScriptableObject
    {
        public List<EnemyConfig> EnemyWave;
        public float DelayBetweenSpawns = 1f;
    }
}
