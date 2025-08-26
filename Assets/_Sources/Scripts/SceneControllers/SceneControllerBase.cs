using System.Threading;
using Cysharp.Threading.Tasks;
using deVoid.Utils;
using JetBrains.Annotations;
using UnicoCaseStudy.Signal;
using UnicoCaseStudy.UI.Components;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace UnicoCaseStudy.SceneControllers
{
    public abstract class SceneControllerBase : MonoBehaviour
    {
        public Camera SceneCamera;
        public UIContainer UIContainer;
        [SerializeField] protected EventSystem EventSystem;

        [UsedImplicitly]
        public void Awake()
        {
            SetState(SceneManager.GetActiveScene().name == gameObject.scene.name);

            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        [UsedImplicitly]
        public void OnDestroy()
        {
            Dispose();
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

        public virtual async UniTask Initialize(CancellationToken cancellationToken)
        {
            await UIContainer.Initialize(cancellationToken);
        }

        public virtual async UniTask Activate(CancellationToken cancellationToken)
        {
            await UIContainer.Activate(cancellationToken);

            Signals.Get<UIContainerChangedSignal>().Dispatch(new UIContainerChangedSignalProperties(UIContainer));
        }

        public virtual UniTask Deactivate(CancellationToken cancellationToken)
        {
            UIContainer?.Deactivate();
            SetState(false);

            return UniTask.CompletedTask;
        }

        protected virtual void Dispose()
        {
        }

        private void OnActiveSceneChanged(Scene fromScene, Scene toScene)
        {
            SetState(toScene.name == gameObject.scene.name);
        }

        private void SetState(bool isActive)
        {
            EventSystem?.gameObject.SetActive(isActive);
            SceneCamera?.gameObject.SetActive(isActive);
            UIContainer?.gameObject.SetActive(isActive);
        }
    }
}