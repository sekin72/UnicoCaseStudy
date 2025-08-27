using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnicoCaseStudy.Gameplay.Systems;
using UnicoCaseStudy.Managers.Gameplay;
using UnicoCaseStudy.UI.Components;
using UnicoCaseStudy.Utilities.MonoBehaviourUtilities;
using UnityEngine;

namespace UnicoCaseStudy.SceneControllers
{
    public class GameplaySceneController : SceneControllerBase
    {
        public EasyInputManager EasyInputManager;
        public List<DefenceSelectorUI> DefenceItemSelectors;
        public DefenceSelectorMover DefenceSelectorMover;

        [SerializeField] private GameObject _light;
        [SerializeField] private CFButton _pauseButton;

        public override async UniTask Activate(CancellationToken cancellationToken)
        {
            await base.Activate(cancellationToken);

            _pauseButton.onClick.RemoveAllListeners();
            _light.SetActive(true);

            await AppManager.GetManager<GameplayManager>().CreateGameplay(this);

            _pauseButton.onClick.AddListener(() => AppManager.GetManager<GameplayManager>().OpenPausePopup());
        }

        public override UniTask Deactivate(CancellationToken cancellationToken)
        {
            _light.SetActive(false);

            AppManager.GetManager<GameplayManager>().Deactivate();

            return base.Deactivate(cancellationToken);
        }
    }
}