using System;
using UnityEngine;

namespace UnicoCaseStudy.Configs
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "UnicoCaseStudy/Configs/GameSettings", order = 3)]
    public class GameSettings : ScriptableObject
    {
        public int TotalWidth = 20;
        public int TotalHeight = 20;

        public int GameplayWidth = 4;
        public int GameplayHeight = 8;

        public float BoardYOffset = 2;

        public int DefencePlaceHeight = 4;

        public BackgroundSpriteWrapper LightGreenSpriteWrapper;
        public BackgroundSpriteWrapper DarkGreenSpriteWrapper;
        public BackgroundSpriteWrapper GreySpriteWrapper;
    }

    [Serializable]
    public struct BackgroundSpriteWrapper
    {
        public Sprite Top;
        public Sprite Middle;
        public Sprite Bottom;
        public Sprite BG;
    }
}