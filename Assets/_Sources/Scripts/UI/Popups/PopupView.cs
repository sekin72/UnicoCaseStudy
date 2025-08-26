using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnicoCaseStudy.MVC;
using UnicoCaseStudy.UI.Components;
using UnityEngine;

namespace UnicoCaseStudy.UI.Popups
{
    public abstract class PopupView : View
    {
        public event Action CloseButtonClicked;

        public Transform Root;
        [SerializeField] private CFButton _closeButton;

        public override UniTask Initialize(CancellationToken cancellationToken)
        {
            SetRootTransform();
            _closeButton.onClick.AddListener(OnCloseButtonClicked);

            return UniTask.CompletedTask;
        }

        public override UniTask Activate(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public override void Deactivate()
        {
            _closeButton.onClick.RemoveAllListeners();
        }
        public override void Dispose()
        {
        }

        private void SetRootTransform()
        {
            if (Root == null)
            {
                Root = transform;
            }
        }

        public void SetVisible(bool isVisible)
        {
            gameObject?.SetActive(isVisible);
        }

        public void SetCloseButtonVisible(bool isVisible)
        {
            _closeButton.gameObject.SetActive(isVisible);
        }

        private void OnCloseButtonClicked()
        {
            CloseButtonClicked?.Invoke();
        }
    }
}