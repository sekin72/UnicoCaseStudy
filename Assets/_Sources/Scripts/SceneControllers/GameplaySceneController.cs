using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnicoCaseStudy.Gameplay;
using UnicoCaseStudy.Gameplay.UI.Popups.Pause;
using UnicoCaseStudy.Managers.Loading;
using UnicoCaseStudy.Managers.Pool;
using UnicoCaseStudy.Managers.UI;
using UnicoCaseStudy.UI.Components;
using UnityEngine;

namespace UnicoCaseStudy.SceneControllers
{
    public class GameplaySceneController : SceneControllerBase
    {
        public event Action Tick;

        public event Action LateTick;

        private PopupManager _popupManager;
        private SceneLoadingManager _loadingManager;
        private PoolManager _poolManager;
        private PoolWarmUpManager _poolWarmUpManager;

        private GameSession _session;
        private IDisposable _messageSubscription;
        private CancellationTokenSource _oldCancellationTokenSource;
        private CancellationToken _originalCancellationToken;

        [SerializeField] private GameObject _light;
        [SerializeField] private CFButton _pauseButton;

        public override async UniTask Activate(CancellationToken cancellationToken)
        {
            _popupManager = AppManager.GetManager<PopupManager>();
            _loadingManager = AppManager.GetManager<SceneLoadingManager>();
            _poolManager = AppManager.GetManager<PoolManager>();
            _poolWarmUpManager = AppManager.GetManager<PoolWarmUpManager>();

            _originalCancellationToken = cancellationToken;

            await base.Activate(cancellationToken);

            _light.SetActive(true);

            await LoadLevel();
        }

        public override UniTask Deactivate(CancellationToken cancellationToken)
        {
            _session?.Dispose();
            _session = null;

            _messageSubscription?.Dispose();
            _messageSubscription = null;

            _light.SetActive(false);
            _popupManager.Close();

            return base.Deactivate(cancellationToken);
        }

        private async UniTask LoadLevel()
        {
            _pauseButton.onClick.RemoveAllListeners();

            if (_oldCancellationTokenSource != null)
            {
                _oldCancellationTokenSource.Cancel();
                _oldCancellationTokenSource.Dispose();
            }

            _session?.Dispose();
            _session = null;

            await LoadLevelInternal();
        }

        private async UniTask LoadLevelInternal()
        {
            _oldCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_originalCancellationToken);
            var linkedCancellationToken = _oldCancellationTokenSource.Token;

            if (!_poolWarmUpManager.LevelWarmUpCompleted)
            {
                await _poolWarmUpManager.StartRemainingJobsForceful(linkedCancellationToken);
            }

            _session = _poolManager.GetGameObject(PoolKeys.GameSession).GetComponent<GameSession>();

            await _session.Initialize(this, linkedCancellationToken);

            _pauseButton.onClick.AddListener(() => OpenPausePopup(linkedCancellationToken));

            await _session.Activate();
        }

        private void OpenPausePopup(CancellationToken cancellationToken)
        {
            _session.PauseGame();
            _popupManager.Open<PausePopup, PausePopupData, PausePopupView>(new PausePopupData(null, RestartLevel, ReturnToMainScene, _session.ResumeGame),
                cancellationToken).Forget();
        }

        public void ReturnToMainScene()
        {
            _session.Dispose();
            _session = null;

            _loadingManager.LoadMainScene().Forget();
        }

        public void RestartLevel()
        {
            LoadLevel().Forget();
        }

        [UsedImplicitly]
        private void Update()
        {
            Tick?.Invoke();

            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartLevel();
            }
        }

        [UsedImplicitly]
        private void LateUpdate()
        {
            LateTick?.Invoke();
        }
    }
}