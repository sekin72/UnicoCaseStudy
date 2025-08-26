using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnicoCaseStudy.UI.Components;
using UnityEngine;

namespace UnicoCaseStudy.UI.Popups.Win
{
    public class WinPopupView : PopupView
    {
        [SerializeField] protected CFButton BackToMainMenuButton;

        public event Action BackToMainMenuButtonClicked;

        public override async UniTask Initialize(CancellationToken cancellationToken)
        {
            await base.Initialize(cancellationToken);

            BackToMainMenuButton.onClick.AddListener(() => BackToMainMenuButtonClicked?.Invoke());
        }

        public override void Deactivate()
        {
            BackToMainMenuButton.onClick.RemoveListener(() => BackToMainMenuButtonClicked?.Invoke());

            base.Deactivate();
        }
    }
}