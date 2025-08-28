using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using deVoid.Utils;
using GameClient.GameData;
using UnicoCaseStudy.Gameplay.Logic;
using UnicoCaseStudy.Gameplay.Signal;
using UnicoCaseStudy.Managers.Gameplay;
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

        private const string _gameplayTileLayerName = "GameplayTile";
        private static int _gameplayTileLayerMask;

        private Camera _camera;

        private DefenceSelectorUI _toBePlacedUI;
        private DefenderConfig _placedDefender;

        public override UniTask Activate(CancellationToken cancellationToken)
        {
            _gameplayTileLayerMask = 1 << LayerMask.NameToLayer(_gameplayTileLayerName);

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
                var tile = hit.Value.collider.GetComponentInParent<GameplayTile>();
                _toBePlacedUI.OnDefenderPlaced();
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

            Ray ray = _camera.ScreenPointToRay(eventData.position);
            RaycastHit2D hitInfo = Physics2D.GetRayIntersection(ray, 1000f, _gameplayTileLayerMask);

            if (hitInfo.collider != null)
            {
                hit = hitInfo;
                return true;
            }

            return false;
        }
    }
}
