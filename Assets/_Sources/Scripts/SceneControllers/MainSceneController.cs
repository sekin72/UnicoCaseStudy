using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnicoCaseStudy.Managers.Data;
using UnicoCaseStudy.Managers.Data.Storages;
using UnicoCaseStudy.Managers.Loading;
using UnicoCaseStudy.Managers.Sound;
using UnicoCaseStudy.Managers.UI;
using UnicoCaseStudy.UI.Components;
using UnicoCaseStudy.UI.Popups;
using UnicoCaseStudy.UI.Popups.Settings;
using UnityEngine;

namespace UnicoCaseStudy.SceneControllers
{
    public class MainSceneController : SceneControllerBase
    {
        protected PopupManager PopupManager;
        private SceneLoadingManager _loadingManager;
        private SoundManager _soundManager;
        private DataManager _dataManager;

        [SerializeField] private CFButton _newGameButton;
        [SerializeField] private CFButton _loadButton;
        [SerializeField] private CFButton _settingsButton;

        public override async UniTask Initialize(CancellationToken cancellationToken)
        {
            await base.Initialize(cancellationToken);

            PopupManager = AppManager.GetManager<PopupManager>();
            _loadingManager = AppManager.GetManager<SceneLoadingManager>();
            _soundManager = AppManager.GetManager<SoundManager>();
            _dataManager = AppManager.GetManager<DataManager>();
        }

        public override UniTask Activate(CancellationToken cancellationToken)
        {
            _loadButton.interactable = _dataManager.Load<GameSessionSaveStorage>() is { GameplayFinished: false };

            _newGameButton.onClick.AddListener(OnNewGameButtonClick);
            _loadButton.onClick.AddListener(OnLoadButtonClick);
            _settingsButton.onClick.AddListener(OnSettingsButtonClick);

            return base.Activate(cancellationToken);
        }

        public override UniTask Deactivate(CancellationToken cancellationToken)
        {
            _soundManager.StopAll();

            _newGameButton.onClick.RemoveListener(OnNewGameButtonClick);
            _loadButton.onClick.RemoveListener(OnLoadButtonClick);
            _settingsButton.onClick.RemoveListener(OnSettingsButtonClick);

            return base.Deactivate(cancellationToken);
        }

        private void OnNewGameButtonClick()
        {
            _dataManager.Save(new GameSessionSaveStorage
            {
                GameplayFinished = false,
                LevelRandomSeed = Mathf.Abs((int)DateTime.Now.Ticks)
            });

            _loadingManager.LoadLevelScene().Forget();
        }

        private void OnLoadButtonClick()
        {
            _loadingManager.LoadLevelScene().Forget();
        }

        private void OnSettingsButtonClick()
        {
            PopupManager.Open<SettingsPopup, SettingsPopupData, SettingsPopupView>(new SettingsPopupData(), this.GetCancellationTokenOnDestroy()).Forget();
        }
    }
}