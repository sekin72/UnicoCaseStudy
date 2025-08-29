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

        public void Initialize(
            GameplayTile attachedGameplayTile,
            Sprite overrideSprite,
            GameObject idleVFX)
        {
            _sprite = _spriteRenderer.sprite;

            IdleVFX = idleVFX;
            IdleVFX.transform.SetParent(_idleVFXParent);
            IdleVFX.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));

            SetAttachedGameplayTile(attachedGameplayTile);

            _sprite = overrideSprite;
            _spriteRenderer.sprite = _sprite;
        }

        public void Dispose()
        {
            AttachedGameplayTile = null;
        }

        private void UpdateSortingOrder()
        {
            _spriteRenderer.sortingOrder = AttachedGameplayTile.BaseSortingOrder + AttachedGameplayTile.AdditionalSortingOrder + 10;

            foreach (var renderer in IdleVFX.GetComponentsInChildren<ParticleSystemRenderer>())
            {
                renderer.sortingOrder = _spriteRenderer.sortingOrder - 1;
            }
        }

        public void SetAttachedGameplayTile(GameplayTile gameplayTile)
        {
            AttachedGameplayTile = gameplayTile;
            UpdateSortingOrder();
        }
    }
}
