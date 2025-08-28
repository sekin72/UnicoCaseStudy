using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UnicoCaseStudy.Gameplay.Logic
{
    public class GameplayTile : MonoBehaviour, IDisposable
    {
        public Tile AttachedTile;
        [ReadOnly] public Vector2Int GameplayIndex;

        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private BoxCollider2D _boxCollider;
        private Sprite _sprite;

        public int BaseSortingOrder { get; private set; }
        public int AdditionalSortingOrder { get; private set; }

        public void Initialize(Tile tile, Vector2Int gameplayIndex, int baseSortingOrder, int additionalSortingOrder = 0, Sprite overrideSprite = null)
        {
            AttachedTile = tile;
            GameplayIndex = gameplayIndex;
            BaseSortingOrder = baseSortingOrder;
            AdditionalSortingOrder = additionalSortingOrder;
            _sprite = _spriteRenderer.sprite;

            if (overrideSprite != null)
            {
                _sprite = overrideSprite;
                _spriteRenderer.sprite = _sprite;
            }

            UpdateSortingOrder();
        }

        public void Dispose()
        {
        }

        private void UpdateSortingOrder()
        {
            if (_spriteRenderer != null)
            {
                _spriteRenderer.sortingOrder = BaseSortingOrder + AdditionalSortingOrder;
            }
        }
    }
}
