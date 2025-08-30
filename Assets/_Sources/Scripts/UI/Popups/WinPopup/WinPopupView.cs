using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnicoCaseStudy.UI.Components;
using UnityEngine;

namespace UnicoCaseStudy.UI.Popups.Win
{
    public class WinPopupView : PopupView
    {
        [SerializeField] protected CFButton NextLevelButton;
        [SerializeField] protected CFButton CloseButton;
        public event Action CloseButtonClicked;
        public event Action NextLevelButtonClicked;

        public override async UniTask Initialize(CancellationToken cancellationToken)
        {
            await base.Initialize(cancellationToken);

            CloseButton.onClick.AddListener(() => CloseButtonClicked?.Invoke());
            NextLevelButton.onClick.AddListener(() => NextLevelButtonClicked?.Invoke());
        }

        public override void Deactivate()
        {
            CloseButton.onClick.RemoveListener(() => CloseButtonClicked?.Invoke());
            NextLevelButton.onClick.RemoveListener(() => NextLevelButtonClicked?.Invoke());

            base.Deactivate();
        }
    }
}