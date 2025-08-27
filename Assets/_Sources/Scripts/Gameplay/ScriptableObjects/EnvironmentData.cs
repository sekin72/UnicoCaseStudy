using UnityEngine;

namespace GameClient.GameData
{
    [CreateAssetMenu(fileName = "EnvironmentData", menuName = "GameClient/EnvironmentData", order = 1)]
    public class EnvironmentData : ScriptableObject
    {
        public int TileSize = 2;

        public int TotalWidth = 20;
        public int TotalHeight = 20;

        public int GameplayWidth = 4;
        public int GameplayHeight = 8;
    }
}
