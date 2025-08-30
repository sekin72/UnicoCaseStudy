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
        [SerializeField] protected CFButton MMButton;

        public event Action RestartButtonClicked;
        public event Action MMButtonClicked;

        public override async UniTask Initialize(CancellationToken cancellationToken)
        {
            await base.Initialize(cancellationToken);

            RestartButton.onClick.AddListener(() => RestartButtonClicked?.Invoke());
            MMButton.onClick.AddListener(() => MMButtonClicked?.Invoke());
        }

        public override void Dispose()
        {
            MMButton.onClick.RemoveAllListeners();
            RestartButton.onClick.RemoveAllListeners();

            base.Dispose();
        }
    }
}