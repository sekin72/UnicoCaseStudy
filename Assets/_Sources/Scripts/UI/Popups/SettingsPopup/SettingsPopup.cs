using System.Threading;
using Cysharp.Threading.Tasks;
using UnicoCaseStudy.Managers.Sound;
using UnicoCaseStudy.Managers.Vibration;

namespace UnicoCaseStudy.UI.Popups.Settings
{
    public class SettingsPopup : Popup<SettingsPopupData, SettingsPopupView>
    {
        private SettingsManager _settingsManager;

        private SoundManager _soundManager;
        private VibrationManager _vibrationManager;

        public override async UniTask Initialize(CancellationToken cancellationToken)
        {
            await base.Initialize(cancellationToken);

            _settingsManager = AppManager.GetManager<SettingsManager>();
            _soundManager = AppManager.GetManager<SoundManager>();
            _vibrationManager = AppManager.GetManager<VibrationManager>();

            View.SoundToggled += OnSoundToggled;
            View.SoundVolumeChanged += OnSoundVolumeChanged;
            View.VibrationToggled += OnVibrationToggled;

            View.SetSound(_settingsManager.IsSoundActive());
            View.SetVolume(_settingsManager.GetVolume());
            View.SetVibration(_settingsManager.IsVibrationActive());

            View.CloseButtonClicked += OnCloseClicked;
        }

        public override void Dispose()
        {
            View.CloseButtonClicked -= OnCloseClicked;

            View.SoundToggled -= OnSoundToggled;
            View.SoundVolumeChanged -= OnSoundVolumeChanged;
            View.VibrationToggled -= OnVibrationToggled;

            base.Dispose();
        }

        private void OnSoundToggled(bool isOn)
        {
            _settingsManager.SetSoundActive(isOn);

            _soundManager.SetSoundActive(isOn);
            View.SetSound(_settingsManager.IsSoundActive());
            View.SetVolume(_settingsManager.GetVolume());
        }

        private void OnSoundVolumeChanged(float value)
        {
            _settingsManager.SetSoundVolume(value);

            _soundManager.SetSoundVolume(value);
            View.SetVolume(_settingsManager.GetVolume());
        }

        private void OnVibrationToggled(bool isOn)
        {
            _settingsManager.SetVibrationActive(isOn);
            _vibrationManager.SetVibrationActive(isOn);
            View.SetVibration(_settingsManager.IsVibrationActive());
        }
    }
}