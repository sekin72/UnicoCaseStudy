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
    [CreateAssetMenu(fileName = "DefenceSelectorSystem", menuName = "UnicoCaseStudy/Systems/DefenceSelectorSystem", order = 5)]
    public class DefenceSelectorSystem : GameSystem
    {
        [SerializeField] private List<DefenderConfig> _defenderConfigs;
        private List<DefenceSelectorUI> _defenceItemSelectors;

        private DefenceSelectorMover _defenceSelectorMover;

        [SerializeField] private LayerMask _gameplayTileLayerMask;

        private Camera _camera;

        private DefenceSelectorUI _toBePlacedUI;
        private DefenderConfig _placedDefender;

        private PoolManager _poolManager;

        private List<BoardItem> _placedDefenders;

        public override UniTask Activate(CancellationToken cancellationToken)
        {
            _poolManager = AppManager.GetManager<PoolManager>();

            _placedDefenders = new();
            var gameplaySceneController = AppManager.GetManager<GameplayManager>().GameplaySceneController;
            _defenceSelectorMover = gameplaySceneController.DefenceSelectorMover;
            _camera = gameplaySceneController.SceneCamera;
            _defenceItemSelectors = gameplaySceneController.DefenceItemSelectors;

            Signals.Get<DefenceItemSelectedSignal>().AddListener(OnDefenceUIClicked);

            for (int i = 0; i < _defenderConfigs.Count; i++)
            {
                _defenceItemSelectors[i].Initialize(_defenderConfigs[i], 5);
            }

            return UniTask.CompletedTask;
        }

        public override void Deactivate()
        {
            Signals.Get<DefenceItemSelectedSignal>().RemoveListener(OnDefenceUIClicked);

            foreach (var selector in _defenceItemSelectors)
            {
                selector.Dispose();
            }

            foreach (var defender in _placedDefenders)
            {
                defender.Dispose();
                _poolManager.SafeReleaseObject(PoolKeys.BoardItem, defender.gameObject);
            }
        }

        public override void Dispose()
        {
        }

        public void OnDefenceUIClicked(DefenceSelectorUI defenceSelectorUI)
        {
            _toBePlacedUI = defenceSelectorUI;
            _placedDefender = defenceSelectorUI.DefenderConfig;
            _defenceSelectorMover.Activate(_placedDefender.Sprite, defenceSelectorUI.transform.position);
        }

        public void OnInputMoved(PointerEventData eventData)
        {
            if (_placedDefender == null)
            {
                return;
            }

            _defenceSelectorMover.MoveTo(eventData.position);
        }

        public void OnInputReleased(PointerEventData eventData)
        {
            if (_placedDefender == null)
            {
                return;
            }

            if (HitGameplayTile(eventData, out var hit) && hit.HasValue)
            {
                TryPlaceDefender(hit.Value);
            }

            _defenceSelectorMover.Deactivate();
            _placedDefender = null;
        }

        private bool HitGameplayTile(PointerEventData eventData, out RaycastHit2D? hit)
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

        private void TryPlaceDefender(RaycastHit2D hit)
        {
            if (!hit.collider.transform.parent.TryGetComponent<GameplayTile>(out var gameplayTile))
            {
                return;
            }

            if (gameplayTile.GameplayIndex.y >= Session.GameSettings.DefencePlaceHeight)
            {
                return;
            }

            _toBePlacedUI.OnDefenderPlaced();

            var defenderBoardItem = _poolManager.GetGameObject(PoolKeys.BoardItem).GetComponent<BoardItem>();

            defenderBoardItem.transform.SetParent(gameplayTile.transform);
            defenderBoardItem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
            defenderBoardItem.transform.localScale = Vector3.one;
            defenderBoardItem.Initialize(gameplayTile,
                                        gameplayTile.BaseSortingOrder +
                                        gameplayTile.AdditionalSortingOrder + 10, 0, _placedDefender.Sprite);

            _placedDefenders.Add(defenderBoardItem);
        }
    }
}
