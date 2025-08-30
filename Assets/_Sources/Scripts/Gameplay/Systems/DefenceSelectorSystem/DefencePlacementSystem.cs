using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using deVoid.Utils;
using GameClient.GameData;
using UnicoCaseStudy.Gameplay.Logic;
using UnicoCaseStudy.Gameplay.Signal;
using UnicoCaseStudy.Managers.Gameplay;
using UnicoCaseStudy.Managers.Pool;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnicoCaseStudy.Gameplay.Systems
{
    [CreateAssetMenu(fileName = "DefencePlacementSystem", menuName = "UnicoCaseStudy/Systems/DefencePlacementSystem", order = 5)]
    public class DefencePlacementSystem : GameSystem
    {
        private List<DefenceSelectorUI> _defenceItemSelectors;
        private DefenceSelectorMover _defenceSelectorMover;

        [SerializeField] private LayerMask _gameplayTileLayerMask;

        private Camera _camera;

        private PoolManager _poolManager;

        private List<Defender> _placedDefenders;

        private PlacementInputTypes _placementInputType;
        private NewPlacementInput _newPlacementInput;
        private SwapPlacementInput _swapPlacementInput;

        public override UniTask Activate(CancellationToken cancellationToken)
        {
            _poolManager = AppManager.GetManager<PoolManager>();

            _placedDefenders = new();
            var gameplaySceneController = AppManager.GetManager<GameplayManager>().GameplaySceneController;
            _defenceSelectorMover = gameplaySceneController.DefenceSelectorMover;
            _camera = gameplaySceneController.SceneCamera;
            _defenceItemSelectors = gameplaySceneController.DefenceItemSelectors;

            Signals.Get<DefenceItemSelectedSignal>().AddListener(OnDefenceUISelected);
            Signals.Get<DefenceItemReleasedSignal>().AddListener(OnDefenceUIReleased);

            var i = 0;
            foreach (var defenderConfigPair in Session.LevelConfig.DefenderCount)
            {
                _defenceItemSelectors[i++].Initialize(Session.LevelConfig.GetDefenderConfig(defenderConfigPair.Key), defenderConfigPair.Value);
            }

            _placementInputType = PlacementInputTypes.None;
            _newPlacementInput = new NewPlacementInput(this, _defenceSelectorMover);
            _swapPlacementInput = new SwapPlacementInput(this);

            return UniTask.CompletedTask;
        }

        public override void Deactivate()
        {
            _defenceSelectorMover.Deactivate();

            Signals.Get<DefenceItemSelectedSignal>().RemoveListener(OnDefenceUISelected);
            Signals.Get<DefenceItemReleasedSignal>().RemoveListener(OnDefenceUIReleased);

            foreach (var selector in _defenceItemSelectors)
            {
                selector.Dispose();
            }

            foreach (var defender in _placedDefenders)
            {
                defender.Dispose();
                _poolManager.SafeReleaseObject(PoolKeys.Defender, defender.gameObject);
            }
        }

        public override void Dispose()
        {
        }

        private void OnDefenceUISelected(DefenceSelectorUI defenceSelectorUI, PointerEventData eventData)
        {
            if (_placementInputType != PlacementInputTypes.None)
            {
                return;
            }

            _placementInputType = PlacementInputTypes.PlacingDefender;
            _newPlacementInput.OnPointerDown(defenceSelectorUI, eventData);
        }

        public void OnInputSelected(PointerEventData eventData)
        {
            if (_placementInputType == PlacementInputTypes.PlacingDefender)
            {
                return;
            }

            if (!HitGameplayTile(eventData, out var hit) || !hit.HasValue)
            {
                _swapPlacementInput.OnPointerUp(eventData);
                _placementInputType = PlacementInputTypes.None;
                return;
            }

            _placementInputType = PlacementInputTypes.SwappingDefender;
            if (_swapPlacementInput.OnPointerDown(eventData))
            {
                _placementInputType = PlacementInputTypes.None;
            }
        }

        public void OnInputMoved(PointerEventData eventData)
        {
            switch (_placementInputType)
            {
                case PlacementInputTypes.PlacingDefender:
                    _newPlacementInput.OnPointerMoved(eventData);
                    break;
                case PlacementInputTypes.SwappingDefender:
                    _swapPlacementInput.OnPointerMoved(eventData);
                    break;
                case PlacementInputTypes.None:
                default:
                    break;
            }
        }

        private void OnDefenceUIReleased(PointerEventData eventData)
        {
            switch (_placementInputType)
            {
                case PlacementInputTypes.PlacingDefender:
                    _newPlacementInput.OnPointerUp(eventData);
                    break;
                case PlacementInputTypes.SwappingDefender:
                    _swapPlacementInput.OnPointerUp(eventData);
                    break;
                case PlacementInputTypes.None:
                default:
                    break;
            }

            _placementInputType = PlacementInputTypes.None;
        }

        public bool HitGameplayTile(PointerEventData eventData, out RaycastHit2D? hit)
        {
            hit = null;

            if (_camera == null)
            {
                return false;
            }

            Vector2 screenPoint = eventData.position;
            Vector2 worldPoint = _camera.ScreenToWorldPoint(screenPoint);

            RaycastHit2D hitInfo = Physics2D.Raycast(worldPoint, Vector2.zero, 0f, _gameplayTileLayerMask);

            if (hitInfo.collider != null)
            {
                hit = hitInfo;
                return true;
            }

            return false;
        }

        public bool CanPlaceDefender(GameplayTile gameplayTile, bool canPlaceOnAnother)
        {
            if (gameplayTile.GameplayIndex.y >= Session.GameSettings.DefencePlaceHeight)
            {
                return false;
            }

            if (!canPlaceOnAnother && gameplayTile.OccupyingDefender != null)
            {
                return false;
            }

            return true;
        }

        public void PlaceDefender(GameplayTile gameplayTile, DefenderConfig defenderConfig)
        {
            var defenderBoardItem = _poolManager.GetGameObject(PoolKeys.Defender).GetComponent<Defender>();
            var idleVFX = _poolManager.GetGameObject(defenderConfig.IdleVFXPoolKey);
            defenderBoardItem.transform.SetParent(gameplayTile.transform);
            defenderBoardItem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
            defenderBoardItem.transform.localScale = Vector3.one;
            defenderBoardItem.Initialize(defenderConfig, gameplayTile, idleVFX);

            gameplayTile.SetOccupyingDefender(defenderBoardItem);
            _placedDefenders.Add(defenderBoardItem);
        }

        private enum PlacementInputTypes
        {
            None,
            PlacingDefender,
            SwappingDefender
        }
    }
}
