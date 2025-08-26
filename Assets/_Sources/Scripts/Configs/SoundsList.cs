using System.Collections.Generic;
using UnityEngine;

namespace UnicoCaseStudy.Configs
{
    [CreateAssetMenu(fileName = "SoundsList", menuName = "UnicoCaseStudy/Configs/SoundsList", order = 1)]
    public class SoundsList : ScriptableObject
    {
        public List<AudioClip> AudioClips;
    }
}