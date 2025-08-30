using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using FranticCase.ScriptableObjects;
using TMPro;
using UnicoCaseStudy.Managers.Data;
using UnicoCaseStudy.Managers.Data.Storages;
using UnicoCaseStudy.Managers.Loading;
using UnicoCaseStudy.Managers.Sound;
using UnicoCaseStudy.Managers.UI;
using UnicoCaseStudy.UI.Components;
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

        [SerializeField] private LevelConfigHolder _levelConfigHolder;
        [SerializeField] private TMP_Dropdown _levelsDropdown;
        [SerializeField] private CFButton _newGameButton;
        [SerializeField] private CFButton _settingsButton;

        private int _selectedLevel;

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
            _soundManager.StopAll();
            _soundManager.PlayOneShot(SoundKeys.MainMenuTheme, playInLoop: true);

            _newGameButton.onClick.AddListener(OnNewGameButtonClick);
            _settingsButton.onClick.AddListener(OnSettingsButtonClick);
            _levelsDropdown.onValueChanged.AddListener(OnSelectedLevelChanged);

            _levelsDropdown.options = new List<TMP_Dropdown.OptionData>();
            for (var i = 0; i < _levelConfigHolder.LevelData.Count; i++)
            {
                _levelsDropdown.options.Add(new TMP_Dropdown.OptionData($"Level {i + 1}"));
            }

            RefreshLevelsDropdown(0);

            return base.Activate(cancellationToken);
        }

        public override UniTask Deactivate(CancellationToken cancellationToken)
        {
            _soundManager.StopAll();

            _newGameButton.onClick.RemoveListener(OnNewGameButtonClick);
            _settingsButton.onClick.RemoveListener(OnSettingsButtonClick);
            _levelsDropdown.onValueChanged.RemoveListener(OnSelectedLevelChanged);

            return base.Deactivate(cancellationToken);
        }

        private void OnNewGameButtonClick()
        {
            _dataManager.Save(new GameSessionSaveStorage
            {
                GameplayFinished = false,
                LevelRandomSeed = Mathf.Abs((int)DateTime.Now.Ticks),
                LevelConfig = _levelConfigHolder.LevelData[_selectedLevel]
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

        private void RefreshLevelsDropdown(int index)
        {
            _levelsDropdown.SetValueWithoutNotify(index);
            _levelsDropdown.value = index;

            _levelsDropdown.RefreshShownValue();
        }

        private void OnSelectedLevelChanged(int index)
        {
            _selectedLevel = index;

            RefreshLevelsDropdown(index);
        }
    }
}