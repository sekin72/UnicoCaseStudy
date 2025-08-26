using System.Threading;
using Cysharp.Threading.Tasks;
using Lofelt.NiceVibrations;
using UnicoCaseStudy.Utilities.Extensions;
using static Lofelt.NiceVibrations.HapticPatterns;

namespace UnicoCaseStudy.Managers.Vibration
{
    public sealed class VibrationManager : Manager
    {
        private static VibrationManager _instance;
        private SettingsManager _settingsManager;
        private IVibrationConfiguration _vibrationConfiguration;

        protected override async UniTask WaitDependencies(CancellationToken disposeToken)
        {
            _settingsManager = AppManager.GetManager<SettingsManager>();

            await UniTask.WaitUntil(() => _settingsManager.IsInitialized,
                cancellationToken: disposeToken);
        }
        protected override UniTask Initialize(CancellationToken disposeToken)
        {
            _instance = this;

            LoadData();

            _vibrationConfiguration = new VibrationEnabledConfiguration();

            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            StopAllHaptics();

            base.Dispose();
        }

        public static void VibrateStatic(VibrationType vibrationType)
        {
            _instance?.Vibrate(vibrationType);
        }

        public void Vibrate(VibrationType vibrationType)
        {
            if (!IsVibrationActive())
            {
                return;
            }

            _vibrationConfiguration.Vibrate(vibrationType.GetPresetType());
        }

        public void SetVibrationActive(bool isActive)
        {
            if (isActive)
            {
                Vibrate(VibrationType.Selection);
            }
        }

        public void TriggerVibration(PresetType hapticType, float seconds, float vibrateAmountForSecond = 1)
        {
            if (!IsVibrationActive())
            {
                return;
            }

            _vibrationConfiguration.TriggerVibration(hapticType, seconds, vibrateAmountForSecond);
        }

        public void StopAllHaptics()
        {
            if (!IsVibrationActive())
            {
                return;
            }

            _vibrationConfiguration.ReleaseVibration();
            HapticController.Stop();
        }

        public bool IsVibrationActive()
        {
            return _settingsManager.IsVibrationActive();
        }
    }
}