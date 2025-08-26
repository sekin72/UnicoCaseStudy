using System.Threading;
using Cysharp.Threading.Tasks;
using UnicoCaseStudy.MVC;
using UnicoCaseStudy.UI.Components;
using UnityEngine;
using UnityEngine.UI;

namespace UnicoCaseStudy.UI.Screens
{
    public abstract class CFScreen<TD, TV> : Controller<TD, TV>, ICFScreen
        where TD : CFScreenData
        where TV : CFScreenView
    {
        public Darkinator Darkinator => View.Darkinator;
        public SafeArea SafeArea => View.SafeArea;

        CFScreenData ICFScreen.Data => Data;
        CFScreenView ICFScreen.View => View;

        public override UniTask Initialize(CancellationToken cancellationToken)
        {
            SafeArea.Initialize();
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)Darkinator.transform);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)SafeArea.transform);

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
    }
}