using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnicoCaseStudy.Managers;
using UnicoCaseStudy.Managers.Asset;
using UnicoCaseStudy.Managers.Data;
using UnicoCaseStudy.Managers.Loading;
using UnityEngine;
using UnityEngine.Pool;

namespace UnicoCaseStudy
{
    public class AppManager : MonoBehaviour
    {
        private static AppManager _instance;

        private IManager[] _managersList;
        private Dictionary<Type, IManager> _managersDictionary;

        private bool _initializationCompleted;

        private void Awake()
        {
            _instance = this;
            UnityEngine.Object.DontDestroyOnLoad(this);

            _managersDictionary = DictionaryPool<Type, IManager>.Get();
            RegisterManagers();
            Initialize().Forget();
        }

        private void RegisterManagers()
        {
            _managersList = GetComponentsInChildren<IManager>(true);
            foreach (var manager in _managersList)
            {
                _managersDictionary.Add(manager.GetType(), manager);
            }
        }

        private async UniTask Initialize()
        {
            await InitializeManagers();

            _initializationCompleted = true;

            await GetManager<SceneLoadingManager>().LoadMainScene();
        }

        private async UniTask InitializeManagers()
        {
            List<UniTask> initializeTasks = new();
            foreach (var manager in _managersList)
            {
                initializeTasks.Add(manager.StartAsync(this.GetCancellationTokenOnDestroy()));
            }

            await UniTask.WhenAll(initializeTasks);
        }

        public static T GetManager<T>() where T : IManager
        {
            if (_instance._managersDictionary.TryGetValue(typeof(T), out var system))
            {
                return (T)system;
            }

            throw new NullReferenceException("Requested Manager is not initialized!");
        }

        private void OnApplicationQuit()
        {
            if(!_initializationCompleted)
            {
                return;
            }

            foreach (var manager in _managersList)
            {
                if(manager is DataManager)
                {
                    continue;
                }

                manager.Dispose();
            }

            GetManager<DataManager>().Dispose();
        }
    }
}
