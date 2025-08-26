using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnicoCaseStudy
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "UnicoCaseStudy/LevelData/LevelConfig", order = 1)]
    public class LevelConfig : ScriptableObject
    {
        public RawImage BGImage;
        public RawImage FGImage;

    }
}
