using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnicoCaseStudy.Managers.Asset;
using UnicoCaseStudy.SceneControllers;
using UnicoCaseStudy.UI.Components;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using ThreadPriority = UnityEngine.ThreadPriority;

namespace UnicoCaseStudy.Managers.Loading
{
    public sealed class SceneLoadingManager : Manager
    {
        [SerializeField] private SceneReferencesHolder _sceneReferencesHolder;

        private const string SceneLoading = "LoadingScene";
        public const string SceneGameplay = "GameplayScene";
        public const string SceneMainMainMenu = "MainMenuScene";

        private readonly Dictionary<string, AssetReference> _sceneReferences = new();
        private readonly Dictionary<string, SceneControllerBase> _sceneControllers = new();
        private readonly Dictionary<string, SceneInstance> _sceneInstances = new();

        private readonly HashSet<string> _scenesBeingLoaded = new();

        private AddressableManager _addressableManager;

        protected override async UniTask WaitDependencies(CancellationToken disposeToken)
        {
            _addressableManager = AppManager.GetManager<AddressableManager>();

            await UniTask.WaitUntil(() => _addressableManager.IsInitialized, cancellationToken: disposeToken);
        }

        protected override async UniTask Initialize(CancellationToken disposeToken)
        {
            _sceneReferences.TryAdd(SceneLoading, _sceneReferencesHolder.LoadingSceneReference);
            _sceneReferences.TryAdd(SceneMainMainMenu, _sceneReferencesHolder.MainMenuSceneReference);
            _sceneReferences.TryAdd(SceneGameplay, _sceneReferencesHolder.GameplaySceneReference);

            var currentScene = SceneManager.GetActiveScene();
            var SceneControllerBase = GetSceneController(currentScene);
            await SceneControllerBase.Initialize(DisposeToken);
            await SceneControllerBase.Activate(DisposeToken);
        }

        private async UniTask LoadScene(string toSceneName)
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            CFButton.IsInputLocked.Increase("SceneLoading");

            Application.backgroundLoadingPriority = ThreadPriority.High;

            await ActivateScene(toSceneName, DisposeToken);

            Application.backgroundLoadingPriority = ThreadPriority.Normal;

            Screen.sleepTimeout = SleepTimeout.SystemSetting;

            CFButton.IsInputLocked.Decrease("SceneLoading");
        }

        public async UniTask<SceneInstance> GetScene(string sceneName, CancellationToken cancellationToken)
        {
            if (_scenesBeingLoaded.Contains(sceneName))
            {
                await UniTask.WaitWhile(
                    () => _scenesBeingLoaded.Contains(sceneName), cancellationToken: cancellationToken
                );
            }

            _sceneInstances.TryGetValue(sceneName, out var sceneInstance);
            var scene = sceneInstance.Scene;

            _sceneInstances.TryGetValue(sceneName, out sceneInstance);

            if (sceneInstance.Scene != default(SceneInstance).Scene)
            {
                return sceneInstance;
            }

            if (!scene.IsValid() || !scene.isLoaded)
            {
                try
                {
                    _scenesBeingLoaded.Add(sceneName);

                    var loadedInstance = await _addressableManager.LoadSceneAdditive(
                        _sceneReferences[sceneName], cancellationToken
                    );
                    _sceneInstances.Add(sceneName, loadedInstance);
                }
                finally
                {
                    _scenesBeingLoaded.Remove(sceneName);
                }

                _sceneInstances.TryGetValue(sceneName, out sceneInstance);
            }

            return sceneInstance;
        }

        private async UniTask<SceneControllerBase> ActivateScene(string sceneName, CancellationToken cancellationToken)
        {
            var sceneInstance = await GetScene(sceneName, cancellationToken);

            await sceneInstance.ActivateAsync().WithCancellation(cancellationToken);

            var SceneControllerBase = GetSceneController(sceneInstance.Scene);
            SceneManager.SetActiveScene(SceneControllerBase.gameObject.scene);
            await SceneControllerBase.Initialize(cancellationToken);
            await SceneControllerBase.Activate(cancellationToken);

            return SceneControllerBase;
        }

        private SceneControllerBase GetSceneController(Scene scene)
        {
            if (_sceneControllers.TryGetValue(scene.name, out var SceneControllerBase))
            {
                return SceneControllerBase;
            }

            foreach (var rootGameObject in scene.GetRootGameObjects())
            {
                if (!rootGameObject.TryGetComponent(out SceneControllerBase))
                {
                    continue;
                }

                _sceneControllers.Add(scene.name, SceneControllerBase);
                return SceneControllerBase;
            }

            if (string.IsNullOrEmpty(scene.name))
            {
                throw new InvalidOperationException(
                    "The scene provided does not have a SceneControllerBase. Did you forget to open an existing scene?"
                );
            }

            throw new InvalidOperationException(
                $"{scene.name} does not have a SceneControllerBase. All scenes must have one."
            );
        }

        public async UniTask LoadLevelScene()
        {
            await LoadScene(SceneGameplay);
        }

        public async UniTask LoadMainScene()
        {
            await LoadScene(SceneMainMainMenu);
        }
    }
}