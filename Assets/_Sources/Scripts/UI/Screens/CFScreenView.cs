using System.Threading;
using Cysharp.Threading.Tasks;
using UnicoCaseStudy.MVC;
using UnicoCaseStudy.UI.Components;

namespace UnicoCaseStudy.UI.Screens
{
    public abstract class CFScreenView : View
    {
        public Darkinator Darkinator;
        public SafeArea SafeArea;

        public override UniTask Initialize(CancellationToken cancellationToken)
        {
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