using System.Threading;
using Cysharp.Threading.Tasks;
using UnicoCaseStudy.Managers.Gameplay;
using UnicoCaseStudy.Utilities.MonoBehaviourUtilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnicoCaseStudy.Gameplay.Systems.InputSystem
{
    [CreateAssetMenu(fileName = "InputSystem", menuName = "UnicoCaseStudy/Systems/InputSystem", order = 4)]
    public sealed class InputSystem : GameSystem
    {
        private DefencePlacementSystem _defenceSelectorSystem;

        private EasyInputManager _easyInputManager;

        public override async UniTask Initialize(GameSession gameSession, CancellationToken cancellationToken)
        {
            await base.Initialize(gameSession, cancellationToken);

            _defenceSelectorSystem = gameSession.GetSystem<DefencePlacementSystem>();
            _easyInputManager = AppManager.GetManager<GameplayManager>().GameplaySceneController.EasyInputManager;
        }

        public override UniTask Activate(CancellationToken cancellationToken)
        {
            SubscribeEvents();
            return UniTask.CompletedTask;
        }

        public override void Deactivate()
        {
            UnsubscribeEvents();
        }

        public override void Dispose()
        {
        }

        private void SubscribeEvents()
        {
            _easyInputManager.Selected += OnInputSelected;
            _easyInputManager.Moved += OnInputMoved;
        }

        private void UnsubscribeEvents()
        {
            _easyInputManager.Selected -= OnInputSelected;
            _easyInputManager.Moved -= OnInputMoved;
        }

        private void OnInputSelected(PointerEventData eventData)
        {
            _defenceSelectorSystem.OnInputSelected(eventData);
        }

        private void OnInputMoved(PointerEventData eventData)
        {
            _defenceSelectorSystem.OnInputMoved(eventData);
        }
    }
}
