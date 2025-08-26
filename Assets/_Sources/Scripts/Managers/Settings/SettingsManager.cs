using System.Threading;
using Cysharp.Threading.Tasks;
using UnicoCaseStudy.Managers;
using UnicoCaseStudy.Managers.Data;
using UnicoCaseStudy.Managers.Data.Storages;
using UnicoCaseStudy.Managers.Vibration;

namespace UnicoCaseStudy
{
    public class SettingsManager : Manager
    {
        private SettingsStorage _settingsStorage;

        private DataManager _dataManager;

        protected override async UniTask WaitDependencies(CancellationToken disposeToken)
        {
            _dataManager = AppManager.GetManager<DataManager>();

            await UniTask.WaitUntil(() => _dataManager.IsInitialized,
                cancellationToken: disposeToken);
        }

        protected override UniTask Initialize(CancellationToken disposeToken)
        {
            LoadData();

            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            SaveData();
            base.Dispose();
        }

        public void SetSoundActive(bool isActive)
        {
            _settingsStorage.IsSoundActive = isActive;
            SaveData();
        }

        public void SetSoundVolume(float value)
        {
            _settingsStorage.MasterVolumeValue = value;
            SaveData();
        }

        public void SetVibrationActive(bool isActive)
        {
            _settingsStorage.IsVibrationActive = isActive;
            SaveData();
        }

        public bool IsSoundActive()
        {
            return _settingsStorage.IsSoundActive;
        }

        public float GetVolume()
        {
            return _settingsStorage.MasterVolumeValue;
        }

        public bool IsVibrationActive()
        {
            return _settingsStorage.IsVibrationActive;
        }

        #region Data

        protected override void LoadData()
        {
            _settingsStorage = _dataManager.Load<SettingsStorage>();
            if (_settingsStorage == null)
            {
                _settingsStorage = new SettingsStorage();
                SaveData();
            }
        }

        protected override void SaveData()
        {
            _dataManager.Save(_settingsStorage);
        }

        #endregion Data
    }
}
