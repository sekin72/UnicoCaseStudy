using System.Collections.Generic;
using UnicoCaseStudy;
using UnityEngine;

namespace FranticCase.ScriptableObjects
{
    [CreateAssetMenu(fileName = "LevelConfigHolder", menuName = "UnicoCaseStudy/GameConfig/LevelConfigHolder", order = 3)]
    public class LevelConfigHolder : ScriptableObject
    {
        public List<LevelConfig> LevelData;
    }
}
