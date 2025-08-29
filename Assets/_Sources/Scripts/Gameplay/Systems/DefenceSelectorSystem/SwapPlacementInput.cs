using UnicoCaseStudy.Gameplay.Logic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnicoCaseStudy.Gameplay.Systems
{
    public class SwapPlacementInput
    {
        private DefencePlacementSystem _defencePlacementSystem;

        private Defender _toBeSwappedBoardItem;
        private GameplayTile _lastHighlighted;

        public SwapPlacementInput(DefencePlacementSystem defencePlacementSystem)
        {
            _defencePlacementSystem = defencePlacementSystem;
        }

        public bool OnPointerDown(PointerEventData eventData)
        {
            _defencePlacementSystem.HitGameplayTile(eventData, out var hit);

            hit.Value.collider.transform.parent.TryGetComponent<Defender>(out var boardItem);
            hit.Value.collider.transform.parent.TryGetComponent<GameplayTile>(out var gameplayTile);

            if (boardItem == null && gameplayTile == null)
            {
                if (_lastHighlighted != null)
                {
                    _lastHighlighted?.SetHighlight(false);
                    _lastHighlighted = null;
                }

                _toBeSwappedBoardItem = null;
                return true;
            }

            if(boardItem == null && gameplayTile.OccupyingDefender != null)
            {
                boardItem = gameplayTile.OccupyingDefender;
            }

            if (_toBeSwappedBoardItem == null && boardItem == null)
            {
                return true;
            }

            if (_toBeSwappedBoardItem == null && boardItem != null)
            {
                _toBeSwappedBoardItem = boardItem;
                TryHighlight(eventData);
                return false;
            }

            if (boardItem != null)
            {
                SwapTwoBoardItems(_toBeSwappedBoardItem, boardItem);
            }
            else if (gameplayTile != null)
            {
                MoveBoardItemTo(_toBeSwappedBoardItem, gameplayTile);
            }

            if (_lastHighlighted != null)
            {
                _lastHighlighted?.SetHighlight(false);
                _lastHighlighted = null;
            }

            _toBeSwappedBoardItem = null;
            return true;
        }

        public void OnPointerMoved(PointerEventData eventData)
        {
            if (_toBeSwappedBoardItem != null)
            {
                TryHighlight(eventData);
                return;
            }

            TryHighlight(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_lastHighlighted != null)
            {
                _lastHighlighted?.SetHighlight(false);
                _lastHighlighted = null;
            }

            _toBeSwappedBoardItem = null;
        }

        private void TryHighlight(PointerEventData pointerEventData)
        {
            if (_toBeSwappedBoardItem == null)
            {
                return;
            }

            if (_defencePlacementSystem.HitGameplayTile(pointerEventData, out var hit) && hit.HasValue)
            {
                if (_lastHighlighted != null)
                {
                    _lastHighlighted?.SetHighlight(false);
                    _lastHighlighted = null;
                }

                GameplayTile gameplayTile = null;

                if (hit.Value.collider.transform.parent.TryGetComponent<BoardItem>(out var boardItem))
                {
                    gameplayTile = boardItem.AttachedGameplayTile;
                }
                else
                {
                    hit.Value.collider.transform.parent.TryGetComponent(out gameplayTile);
                    boardItem = gameplayTile.OccupyingDefender;
                }

                _lastHighlighted = gameplayTile;
                gameplayTile.SetHighlight(true, _defencePlacementSystem.CanPlaceDefender(gameplayTile, true) &&
                    boardItem != _toBeSwappedBoardItem);
            }
            else
            {
                if (_lastHighlighted != null)
                {
                    _lastHighlighted?.SetHighlight(false);
                    _lastHighlighted = null;
                }
            }
        }

        private static void SwapTwoBoardItems(Defender boardItem1, Defender boardItem2)
        {
            if (boardItem1 == boardItem2)
            {
                return;
            }

            var gameplayTile1 = boardItem1.AttachedGameplayTile;
            var gameplayTile2 = boardItem2.AttachedGameplayTile;

            boardItem1.transform.SetParent(gameplayTile2.transform);
            boardItem1.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
            boardItem1.transform.localScale = Vector3.one;

            boardItem2.transform.SetParent(gameplayTile1.transform);
            boardItem2.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
            boardItem2.transform.localScale = Vector3.one;

            gameplayTile1.SetOccupyingDefender(boardItem2);
            gameplayTile2.SetOccupyingDefender(boardItem1);

            boardItem1.SetAttachedGameplayTile(gameplayTile2);
            boardItem2.SetAttachedGameplayTile(gameplayTile1);
        }

        private static void MoveBoardItemTo(Defender boardItem, GameplayTile gameplayTile)
        {
            boardItem.AttachedGameplayTile.SetOccupyingDefender(null);

            boardItem.transform.SetParent(gameplayTile.transform);
            boardItem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
            boardItem.transform.localScale = Vector3.one;
            gameplayTile.SetOccupyingDefender(boardItem);
            boardItem.SetAttachedGameplayTile(gameplayTile);
        }
    }
}
