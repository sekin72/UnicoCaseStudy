using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnicoCaseStudy.Gameplay;
using UnicoCaseStudy.Gameplay.UI.Popups.Pause;
using UnicoCaseStudy.Managers.Loading;
using UnicoCaseStudy.Managers.Pool;
using UnicoCaseStudy.Managers.UI;
using UnicoCaseStudy.SceneControllers;
using UnityEngine;

namespace UnicoCaseStudy.Managers.Gameplay
{
    public class GameplayManager : Manager
    {
        public GameSession GameSession { get; private set; }
        public GameplaySceneController GameplaySceneController { get; private set; }
        public event Action Tick;

        public event Action LateTick;

        private PopupManager _popupManager;
        private SceneLoadingManager _loadingManager;
        private PoolManager _poolManager;
        private PoolWarmUpManager _poolWarmUpManager;

        private CancellationTokenSource _currentCancellationTokenSource;

        private bool _levelActivated;

        protected override UniTask Initialize(CancellationToken disposeToken)
        {
            return UniTask.CompletedTask;
        }

        public async UniTask CreateGameplay(GameplaySceneController gameplaySceneController)
        {
            GameplaySceneController = gameplaySceneController;

            _popupManager = AppManager.GetManager<PopupManager>();
            _loadingManager = AppManager.GetManager<SceneLoadingManager>();
            _poolManager = AppManager.GetManager<PoolManager>();
            _poolWarmUpManager = AppManager.GetManager<PoolWarmUpManager>();

            await LoadLevel();

            _levelActivated = true;
        }

        public void Deactivate()
        {
            _levelActivated = false;

            if (GameSession != null)
            {
                GameSession.Dispose();
                _poolManager.SafeReleaseObject(PoolKeys.GameSession, GameSession.gameObject);
                GameSession = null;
            }

            _popupManager.Close();
        }

        private async UniTask LoadLevel()
        {
            if (_currentCancellationTokenSource != null)
            {
                _currentCancellationTokenSource.Cancel();
                _currentCancellationTokenSource.Dispose();
                _currentCancellationTokenSource = null;
            }

            if (GameSession != null)
            {
                GameSession.Dispose();
                _poolManager.SafeReleaseObject(PoolKeys.GameSession, GameSession.gameObject);
                GameSession = null;
            }

            await LoadLevelInternal();
        }

        private async UniTask LoadLevelInternal()
        {
            _currentCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(DisposeToken);
            var linkedCancellationToken = _currentCancellationTokenSource.Token;

            if (!_poolWarmUpManager.LevelWarmUpCompleted)
            {
                await _poolWarmUpManager.StartRemainingJobsForceful(linkedCancellationToken);
            }

            GameSession = _poolManager.GetGameObject(PoolKeys.GameSession).GetComponent<GameSession>();

            await GameSession.Initialize(linkedCancellationToken);

            await GameSession.Activate();
        }

        public void OpenPausePopup()
        {
            GameSession.PauseGame();
            _popupManager.Open<PausePopup, PausePopupData, PausePopupView>(new PausePopupData(null, RestartLevel, ReturnToMainScene, GameSession.ResumeGame),
                _currentCancellationTokenSource.Token).Forget();
        }

        public void ReturnToMainScene()
        {
            if (GameSession != null)
            {
                GameSession.Dispose();
                _poolManager.SafeReleaseObject(PoolKeys.GameSession, GameSession.gameObject);
                GameSession = null;
            }

            _loadingManager.LoadMainScene().Forget();
        }

        public void RestartLevel()
        {
            LoadLevel().Forget();
        }

        [UsedImplicitly]
        private void Update()
        {
            if (!_levelActivated)
            {
                return;
            }

            Tick?.Invoke();

            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartLevel();
            }
        }

        [UsedImplicitly]
        private void LateUpdate()
        {
            if (!_levelActivated)
            {
                return;
            }

            LateTick?.Invoke();
        }
    }
}
