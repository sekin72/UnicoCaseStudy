using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UnicoCaseStudy.Gameplay.Logic
{
    public class BoardItem : MonoBehaviour, IDisposable
    {
        [ReadOnly] public GameplayTile AttachedGameplayTile;

        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Transform _idleVFXParent;
        public GameObject IdleVFX { get; private set; }
        private Sprite _sprite;
        private int _baseSortingOrder;
        private int _additionalSortingOrder;

        public void Initialize(GameplayTile attachedGameplayTile, int baseSortingOrder, int additionalSortingOrder, Sprite overrideSprite,
            GameObject idleVFX)
        {
            _sprite = _spriteRenderer.sprite;

            AttachedGameplayTile = attachedGameplayTile;
            _baseSortingOrder = baseSortingOrder;
            _additionalSortingOrder = additionalSortingOrder;

            _sprite = overrideSprite;
            _spriteRenderer.sprite = _sprite;

            IdleVFX = idleVFX;
            IdleVFX.transform.SetParent(_idleVFXParent);
            IdleVFX.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));

            UpdateSortingOrder();
        }

        public void Dispose()
        {
            AttachedGameplayTile = null;
        }

        private void UpdateSortingOrder()
        {
            _spriteRenderer.sortingOrder = _baseSortingOrder + _additionalSortingOrder;

            foreach (var renderer in IdleVFX.GetComponentsInChildren<ParticleSystemRenderer>())
            {
                renderer.sortingOrder = _spriteRenderer.sortingOrder - 1;
            }
        }
    }
}
