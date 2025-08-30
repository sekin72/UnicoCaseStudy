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
        public Transform Root;

        public override UniTask Initialize(CancellationToken cancellationToken)
        {
            SetRootTransform();

            return UniTask.CompletedTask;
        }

        public override UniTask Activate(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public override void Deactivate()
        {
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
    }
}