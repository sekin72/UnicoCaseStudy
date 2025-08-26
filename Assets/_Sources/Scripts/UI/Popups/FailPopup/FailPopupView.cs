using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnicoCaseStudy.UI.Components;
using UnityEngine;

namespace UnicoCaseStudy.UI.Popups.Fail
{
    public class FailPopupView : PopupView
    {
        [SerializeField] protected CFButton RestartButton;

        public event Action RestartButtonClicked;

        public override async UniTask Initialize(CancellationToken cancellationToken)
        {
            await base.Initialize(cancellationToken);

            RestartButton.onClick.AddListener(() => RestartButtonClicked?.Invoke());
        }

        public override void Dispose()
        {
            RestartButton.onClick.RemoveAllListeners();

            base.Dispose();
        }
    }
}