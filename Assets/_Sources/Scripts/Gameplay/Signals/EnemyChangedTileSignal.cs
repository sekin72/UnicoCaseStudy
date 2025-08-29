using deVoid.Utils;
using UnityEngine;

namespace UnicoCaseStudy
{
    public class EnemyChangedTileSignal : ASignal<EnemyChangedTileSignalProperties>
    {
    }

    public readonly struct EnemyChangedTileSignalProperties
    {
        public readonly Enemy Enemy;
        public readonly Vector2Int OldTileIndex;
        public readonly Vector2Int NewTileIndex;
        public EnemyChangedTileSignalProperties(Enemy enemy, Vector2Int oldTile, Vector2Int newTile)
        {
            Enemy = enemy;
            OldTileIndex = oldTile;
            NewTileIndex = newTile;
        }
    }
}
