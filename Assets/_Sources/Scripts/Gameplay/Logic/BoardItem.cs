using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UnicoCaseStudy.Gameplay.Logic
{
    public abstract class BoardItem : MonoBehaviour, IDisposable
    {
        [ReadOnly] public GameplayTile AttachedGameplayTile;

        [SerializeField] protected SpriteRenderer _spriteRenderer;
        [SerializeField] protected Transform _idleVFXParent;
        public GameObject IdleVFX { get; protected set; }
        protected Sprite _sprite;

        public virtual void Dispose()
        {
            AttachedGameplayTile = null;
        }

        protected virtual void UpdateSortingOrder()
        {
            _spriteRenderer.sortingOrder = SortingOrderConstants.Defenders;

            foreach (var renderer in IdleVFX.GetComponentsInChildren<ParticleSystemRenderer>())
            {
                renderer.sortingOrder = SortingOrderConstants.DefenderVFX;
            }
        }

        public virtual void SetAttachedGameplayTile(GameplayTile gameplayTile)
        {
            AttachedGameplayTile = gameplayTile;
            UpdateSortingOrder();
        }
    }
}
