using System.Collections.Generic;
using UnicoCaseStudy.Gameplay;
using UnityEngine;

namespace UnicoCaseStudy.Gameplay
{
    [CreateAssetMenu(fileName = "SystemsCollection", menuName = "UnicoCaseStudy/Configs/SystemsCollection", order = 1)]
    public class SystemsCollection : ScriptableObject
    {
        public List<GameSystem> Systems;
    }
}