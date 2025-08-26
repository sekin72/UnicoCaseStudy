using System.Threading;
using Cysharp.Threading.Tasks;
using UnicoCaseStudy.Managers.Data.Storages;
using UnicoCaseStudy.Managers.Data.Syncers;
using UnityEngine;

namespace UnicoCaseStudy.Managers.Data
{
    public class DataManager : Manager
    {
        private const float SaveInterval = 30.0f;

        protected string SaveKey = "Save";
        protected bool IsSaveDirty;
        private float _lastSaveTime;

        private ILocalSyncer<SaveStorage> _localSyncer;
        private SaveStorage _saveStorage;

        public virtual T Load<T>() where T : class, IStorage, new()
        {
            return _saveStorage.Get<T>();
        }

        public virtual void Save<T>(T data) where T : class, IStorage, new()
        {
            _saveStorage.Set(data);
            IsSaveDirty = true;
        }

        private void ForceSave()
        {
            Debug.Log("Force Save");
            SaveAll();
        }

        protected override async UniTask Initialize(CancellationToken disposeToken)
        {
            _localSyncer = new LocalStorageSyncer<SaveStorage>(SaveKey, PlayerPrefs.GetInt("PlayerID").ToString());

            _saveStorage = await _localSyncer.Load(disposeToken);

            StartAutoSavingJob(disposeToken).Forget();
        }

        protected async UniTaskVoid StartAutoSavingJob(CancellationToken cancellationToken)
        {
            Debug.Log("[SaveJob] Start auto save Job");
            while (true)
            {
                await UniTask.WaitUntil(
                    () => IsSaveDirty && _lastSaveTime + SaveInterval < Time.realtimeSinceStartup,
                    PlayerLoopTiming.PostLateUpdate,
                    cancellationToken
                );

                Debug.Log("[SaveJob] Auto save triggered");
                SaveAll();
            }
        }

        private void SaveAll()
        {
            if (!IsSaveDirty)
            {
                return;
            }

            _lastSaveTime = Time.realtimeSinceStartup;
            IsSaveDirty = false;

            SaveLocal();
        }

        protected virtual void SaveLocal()
        {
            _localSyncer.Save(_saveStorage);
        }

        public override void Dispose()
        {
            ForceSave();

            base.Dispose();
        }
    }
}