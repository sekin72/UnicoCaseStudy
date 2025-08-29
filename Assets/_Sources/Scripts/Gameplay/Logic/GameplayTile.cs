using System;
using Sirenix.OdinInspector;
using UnicoCaseStudy.Configs;
using UnityEngine;

namespace UnicoCaseStudy.Gameplay.Logic
{
    public class GameplayTile : MonoBehaviour, IDisposable
    {
        public Tile AttachedTile;
        public Defender OccupyingDefender { get; private set; }
        [ReadOnly] public Vector2Int GameplayIndex;

        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private SpriteRenderer _highlightSpriteRenderer;
        [SerializeField] private BoxCollider2D _boxCollider;
        private Sprite _sprite;

        private Color _positiveColor;
        private Color _negativeColor;

        public void Initialize(
            Tile tile,
            Vector2Int gameplayIndex,
            bool checkerTile,
            GameSettings gameSettings,
            Sprite overrideSprite = null)
        {
            AttachedTile = tile;
            GameplayIndex = gameplayIndex;
            _sprite = _spriteRenderer.sprite;

            if (overrideSprite != null)
            {
                _sprite = overrideSprite;
                _spriteRenderer.sprite = _sprite;
            }

            if (checkerTile)
            {
                Color.RGBToHSV(_spriteRenderer.color, out float h, out float s, out float v);
                v = 0.9f;
                _spriteRenderer.color = Color.HSVToRGB(h, s, v);
            }

            UpdateSortingOrder();

            _highlightSpriteRenderer.gameObject.SetActive(false);

            _positiveColor = gameSettings.PositiveHighlightColor;
            _negativeColor = gameSettings.NegativeHighlightColor;
        }

        public void Dispose()
        {
        }

        private void UpdateSortingOrder()
        {
            _spriteRenderer.sortingOrder = SortingOrderConstants.GameplayTiles;
            _highlightSpriteRenderer.sortingOrder = SortingOrderConstants.GameplayTileHighlights;
        }

        public void SetOccupyingDefender(Defender boardItem)
        {
            OccupyingDefender = boardItem;
        }

        public void SetHighlight(bool highlight, bool impact = true)
        {
            _highlightSpriteRenderer.gameObject.SetActive(highlight);
            if (!highlight)
            {
                return;
            }

            _highlightSpriteRenderer.color = impact ? _positiveColor : _negativeColor;
        }
    }
}
