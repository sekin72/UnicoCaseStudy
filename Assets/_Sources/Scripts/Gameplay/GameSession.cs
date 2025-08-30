using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using deVoid.Utils;
using UnicoCaseStudy.Configs;
using UnicoCaseStudy.Gameplay.Signal;
using UnicoCaseStudy.Gameplay.Systems;
using UnicoCaseStudy.Managers.Data;
using UnicoCaseStudy.Managers.Data.Storages;
using UnicoCaseStudy.Managers.Gameplay;
using UnicoCaseStudy.Managers.Sound;
using UnicoCaseStudy.Managers.UI;
using UnicoCaseStudy.Managers.Vibration;
using UnicoCaseStudy.UI.Popups.Fail;
using UnicoCaseStudy.UI.Popups.Win;
using UnicoCaseStudy.Utilities;
using UnityEngine;
using UnityEngine.Pool;

namespace UnicoCaseStudy.Gameplay
{
    public class GameSession : MonoBehaviour
    {
        private List<ITickable> _tickables;
        private List<ILateTickable> _lateTickables;

        public CancellationTokenSource CancellationTokenSource { get; private set; }
        public LockBin InputDisabled { get; private set; }

        private GameplayManager _gameplayManager;
        private DataManager _dataManager;
        private PopupManager _popupManager;
        private SoundManager _soundManager;
        private VibrationManager _vibrationManager;

        private List<IGameSystem> _gameSystems;
        private Dictionary<Type, IGameSystem> _gameSystemsDictionary;

        [field: SerializeField] public GameSessionSaveStorage GameSessionSaveStorage { get; private set; }
        [field: SerializeField] public GameSettings GameSettings { get; private set; }
        public LevelConfig LevelConfig { get; private set; }
        [SerializeField] private SystemsCollection _systemsCollection;

        private bool _deactivated;
        private bool _disposed;

        public async UniTask Initialize(CancellationToken toBeLinkedToken)
        {
            _gameplayManager = AppManager.GetManager<GameplayManager>();
            _dataManager = AppManager.GetManager<DataManager>();
            _popupManager = AppManager.GetManager<PopupManager>();
            _soundManager = AppManager.GetManager<SoundManager>();
            _vibrationManager = AppManager.GetManager<VibrationManager>();

            _disposed = false;
            _deactivated = false;

            Application.targetFrameRate = 60;

            CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(toBeLinkedToken);
            var cancellationToken = CancellationTokenSource.Token;

            _soundManager.StopAll();

            _gameSystems = ListPool<IGameSystem>.Get();
            _gameSystemsDictionary = DictionaryPool<Type, IGameSystem>.Get();

            InputDisabled = new LockBin();
            _tickables = ListPool<ITickable>.Get();
            _lateTickables = ListPool<ILateTickable>.Get();

            GameSessionSaveStorage = _dataManager.Load<GameSessionSaveStorage>();
            LevelConfig = _gameplayManager.LevelConfig;

            RegisterSystems(_systemsCollection);

            foreach (var system in _gameSystems)
            {
                await system.Initialize(this, cancellationToken);
            }

            Signals.Get<LevelFinishedSignal>().AddListener(LevelFinished);

            ResumeGame();
        }

        public async UniTask Activate()
        {
            var tasks = new List<UniTask>(_gameSystems.Count);
            foreach (var system in _gameSystems)
            {
                tasks.Add(system.Activate(CancellationTokenSource.Token));
            }

            await UniTask.WhenAll(tasks);

            RegisterTicks();
        }

        private void Deactivate()
        {
            if (_deactivated)
            {
                return;
            }

            _deactivated = true;

            Signals.Get<LevelFinishedSignal>().RemoveListener(LevelFinished);

            if (_tickables.Count > 0)
            {
                _gameplayManager.Tick -= Tick;
            }

            if (_lateTickables.Count > 0)
            {
                _gameplayManager.LateTick -= LateTick;
            }

            ListPool<ITickable>.Release(_tickables);
            ListPool<ILateTickable>.Release(_lateTickables);

            for (var i = _gameSystems.Count - 1; i >= 0; i--)
            {
                _gameSystems[i]?.Deactivate();
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            Deactivate();

            CancellationTokenSource?.Cancel();
            CancellationTokenSource?.Dispose();
            CancellationTokenSource = null;

            _disposed = true;

            for (var i = _gameSystems.Count - 1; i >= 0; i--)
            {
                _gameSystems[i]?.Dispose();
            }

            ListPool<IGameSystem>.Release(_gameSystems);
            DictionaryPool<Type, IGameSystem>.Release(_gameSystemsDictionary);
        }

        private void Tick()
        {
            foreach (var tickable in _tickables)
            {
                tickable.Tick();
            }
        }

        private void LateTick()
        {
            foreach (var lateTickable in _lateTickables)
            {
                lateTickable.LateTick();
            }
        }

        private void LevelFinished(bool success)
        {
            if (_deactivated)
            {
                return;
            }

            Deactivate();

            GameSessionSaveStorage.GameplayFinished = true;
            SaveGameSessionStorage();

            if (!success)
            {
                _soundManager.PlayOneShot(SoundKeys.GameOver);
                _vibrationManager.Vibrate(VibrationType.Failure);
                _popupManager.Open<FailPopup, FailPopupData, FailPopupView>(
                    new FailPopupData(_gameplayManager.RestartLevel, _gameplayManager.ReturnToMainScene),
                    CancellationTokenSource.Token).Forget();
                return;
            }

            _soundManager.PlayOneShot(SoundKeys.Success);
            _vibrationManager.Vibrate(VibrationType.Success);
            _popupManager.Open<WinPopup, WinPopupData, WinPopupView>(
                new WinPopupData(_gameplayManager.ReturnToMainScene),
                CancellationTokenSource.Token).Forget();
        }

        private void RegisterTicks()
        {
            if (_tickables.Count > 0)
            {
                _gameplayManager.Tick += Tick;
            }

            if (_lateTickables.Count > 0)
            {
                _gameplayManager.LateTick += LateTick;
            }
        }

        private void RegisterSystems(SystemsCollection systemsCollection)
        {
            foreach (var system in systemsCollection.Systems)
            {
                _gameSystems.Add(system);
                _gameSystemsDictionary.Add(system.GetType(), system);

                if (system is ITickable tickable)
                {
                    _tickables.Add(tickable);
                }

                if (system is ILateTickable lateTickable)
                {
                    _lateTickables.Add(lateTickable);
                }
            }
        }

        public T GetSystem<T>() where T : IGameSystem
        {
            _gameSystemsDictionary.TryGetValue(typeof(T), out var system);
            return (T)system;
        }

        public void PauseGame()
        {
            Time.timeScale = 0;
        }

        public void ResumeGame()
        {
            Time.timeScale = 1;
        }

        public void SaveGameSessionStorage()
        {
            _dataManager.Save(GameSessionSaveStorage);
        }
    }
}