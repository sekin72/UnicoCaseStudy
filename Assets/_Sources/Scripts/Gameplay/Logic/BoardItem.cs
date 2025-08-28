using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UnicoCaseStudy.Gameplay.Logic
{
    public class BoardItem : MonoBehaviour, IDisposable
    {
        [ReadOnly] public GameplayTile AttachedGameplayTile;

        [SerializeField] private SpriteRenderer _spriteRenderer;
        private Sprite _sprite;
        private int _baseSortingOrder;
        private int _additionalSortingOrder;

        public void Initialize(GameplayTile attachedGameplayTile, int baseSortingOrder, int additionalSortingOrder, Sprite overrideSprite)
        {
            _sprite = _spriteRenderer.sprite;

            AttachedGameplayTile = attachedGameplayTile;
            _baseSortingOrder = baseSortingOrder;
            _additionalSortingOrder = additionalSortingOrder;

            _sprite = overrideSprite;
            _spriteRenderer.sprite = _sprite;

            UpdateSortingOrder();
        }

        public void Dispose()
        {
            AttachedGameplayTile = null;
        }

        private void UpdateSortingOrder()
        {
            if (_spriteRenderer != null)
            {
                _spriteRenderer.sortingOrder = _baseSortingOrder + _additionalSortingOrder;
            }
        }
    }
}
