using GameClient.GameData;
using UnicoCaseStudy.Gameplay.Logic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnicoCaseStudy.Gameplay.Systems
{
    public class NewPlacementInput
    {
        private DefencePlacementSystem _defencePlacementSystem;
        private DefenceSelectorMover _defenceSelectorMover;

        private DefenceSelectorUI _toBePlacedUI;
        private DefenderConfig _toBePlacedDefender;
        private GameplayTile _lastHighlighted;

        public NewPlacementInput(DefencePlacementSystem defencePlacementSystem, DefenceSelectorMover defenceSelectorMover)
        {
            _defencePlacementSystem = defencePlacementSystem;
            _defenceSelectorMover = defenceSelectorMover;
        }

        public void OnPointerDown(DefenceSelectorUI defenceSelectorUI, PointerEventData eventData)
        {
            _toBePlacedUI = defenceSelectorUI;
            _toBePlacedDefender = defenceSelectorUI.DefenderConfig;
            _defenceSelectorMover.Activate(_toBePlacedDefender.Sprite, eventData.position);
        }

        public void OnPointerMoved(PointerEventData eventData)
        {
            if (_toBePlacedDefender == null)
            {
                return;
            }

            _defenceSelectorMover.MoveTo(eventData.position);
            TryHighlight(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_toBePlacedDefender == null)
            {
                return;
            }

            if (_defencePlacementSystem.HitGameplayTile(eventData, out var hit) && hit.HasValue)
            {
                TryPlaceDefender(hit.Value);
            }

            _lastHighlighted?.SetHighlight(false);
            _lastHighlighted = null;
            _defenceSelectorMover.Deactivate();
            _toBePlacedDefender = null;
        }

        private void TryPlaceDefender(RaycastHit2D hit)
        {
            if (!hit.collider.transform.parent.TryGetComponent<GameplayTile>(out var gameplayTile))
            {
                return;
            }

            if (!_defencePlacementSystem.CanPlaceDefender(gameplayTile, false))
            {
                return;
            }

            _toBePlacedUI.OnDefenderPlaced();
            _defencePlacementSystem.PlaceDefender(gameplayTile, _toBePlacedDefender);
        }

        public void TryHighlight(PointerEventData pointerEventData)
        {
            if (_defencePlacementSystem.HitGameplayTile(pointerEventData, out var hit) && hit.HasValue)
            {
                if (_lastHighlighted != null)
                {
                    _lastHighlighted?.SetHighlight(false);
                }

                GameplayTile gameplayTile = null;

                if (hit.Value.collider.transform.parent.TryGetComponent<Defender>(out var boardItem))
                {
                    gameplayTile = boardItem.AttachedGameplayTile;
                }
                else
                {
                    hit.Value.collider.transform.parent.TryGetComponent(out gameplayTile);
                }

                _lastHighlighted = gameplayTile;
                gameplayTile.SetHighlight(true, _defencePlacementSystem.CanPlaceDefender(gameplayTile, false));
            }
            else
            {
                _lastHighlighted?.SetHighlight(false);
            }
        }
    }
}
