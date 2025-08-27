using UnityEngine;

namespace UnicoCaseStudy.Gameplay.Logic
{
    public class PassiveTile : MonoBehaviour
    {
        public Tile AttachedTile;

        public void Initialize(Tile tile)
        {
            AttachedTile = tile;
        }
    }
}
