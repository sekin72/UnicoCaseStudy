using System.Collections.Generic;
using GameClient.GameData;
using UnityEngine;
using VInspector;

namespace UnicoCaseStudy
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "UnicoCaseStudy/GameConfig/LevelConfig", order = 1)]
    public class LevelConfig : ScriptableObject
    {
        public List<DefenderConfig> DefenderMap;
        public List<EnemyConfig> EnemyMap;

        public SerializedDictionary<int, int> DefenderCount;
        public SerializedDictionary<int, int> EnemyCount;

        public float DelayBetweenSpawns = 1f;

        public DefenderConfig GetDefenderConfig(int defenderId)
        {
            return DefenderMap[defenderId];
        }

        public EnemyConfig GetEnemyConfig(int enemyID)
        {
            return EnemyMap[enemyID];
        }
    }
}
