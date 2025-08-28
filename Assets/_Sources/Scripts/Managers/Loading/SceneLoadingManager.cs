using System;
using System.Collections.Generic;
using System.Threading;
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
        private const string ScenePreloader = "PreloaderScene";
        public const string SceneLevelScene = "GameplayScene";
        public const string SceneMain = "MainMenuScene";

        private readonly Dictionary<string, SceneControllerBase> _sceneControllers = new();
        private readonly Dictionary<string, SceneInstance> _sceneInstances = new();

        private readonly HashSet<string> _scenesBeingLoaded = new();

        private AddressableManager _addressableManager;

        private Scene _preloaderScene;

        private Dictionary<string, AssetReference> _sceneReferences = new();

        [SerializeField] private SceneReferencesHolder _sceneReferencesHolder;

        protected override async UniTask WaitDependencies(CancellationToken disposeToken)
        {
            _addressableManager = AppManager.GetManager<AddressableManager>();

            await UniTask.WaitUntil(() => _addressableManager.IsInitialized, cancellationToken: disposeToken);
        }

        protected override async UniTask Initialize(CancellationToken disposeToken)
        {
            var currentScene = SceneManager.GetActiveScene();
            var sceneController = GetSceneController(currentScene);
            await sceneController.Initialize(DisposeToken);
            await sceneController.Activate(DisposeToken);

            if (currentScene.name == ScenePreloader)
            {
                _preloaderScene = currentScene;
            }

            _sceneReferences = new Dictionary<string, AssetReference>
            {
                {
                    SceneMain, _sceneReferencesHolder.MainMenuSceneReference
                },
                {
                    SceneLevelScene, _sceneReferencesHolder.GameplaySceneReference
                }
            };
        }

        private async UniTask LoadScene(string toSceneName)
        {
            var cancellationToken = DisposeToken;

            var fromScene = SceneManager.GetActiveScene();
            var fromSceneName = fromScene.name;
            var fromSceneController = GetSceneController(fromScene);

            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            CFButton.IsInputLocked.Increase("SceneLoading");

            Application.backgroundLoadingPriority = ThreadPriority.High;

            var toSceneController = await ActivateScene(toSceneName, cancellationToken);

            Application.backgroundLoadingPriority = ThreadPriority.Normal;

            await UnloadScene(fromSceneName, cancellationToken);

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

            if (sceneName == ScenePreloader)
            {
                scene = _preloaderScene;
            }

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

            var sceneController = GetSceneController(sceneInstance.Scene);
            SceneManager.SetActiveScene(sceneController.gameObject.scene);
            await sceneController.Initialize(cancellationToken);
            await sceneController.Activate(cancellationToken);

            return sceneController;
        }

        private async UniTask UnloadScene(string sceneName, CancellationToken cancellationToken)
        {
            _sceneInstances.TryGetValue(sceneName, out var sceneInstance);
            var scene = sceneInstance.Scene;

            if (sceneName == ScenePreloader)
            {
                scene = _preloaderScene;
            }

            var sceneController = GetSceneController(scene);
            await sceneController.Deactivate(cancellationToken);
        }

        private SceneControllerBase GetSceneController(Scene scene)
        {
            if (_sceneControllers.TryGetValue(scene.name, out var sceneController))
            {
                return sceneController;
            }

            foreach (var rootGameObject in scene.GetRootGameObjects())
            {
                if (!rootGameObject.TryGetComponent(out sceneController))
                {
                    continue;
                }

                _sceneControllers.Add(scene.name, sceneController);
                return sceneController;
            }

            if (string.IsNullOrEmpty(scene.name))
            {
                throw new InvalidOperationException(
                    "The scene provided does not have a SceneController. Did you forget to open an existing scene?"
                );
            }

            throw new InvalidOperationException(
                $"{scene.name} does not have a SceneController. All scenes must have one."
            );
        }

        public async UniTask LoadLevelScene()
        {
            await LoadScene(SceneLevelScene);
        }

        public async UniTask LoadMainScene()
        {
            await LoadScene(SceneMain);
        }
    }
}