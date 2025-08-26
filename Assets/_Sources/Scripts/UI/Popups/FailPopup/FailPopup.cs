using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace UnicoCaseStudy.UI.Popups.Fail
{
    public class FailPopup : Popup<FailPopupData, FailPopupView>
    {
        public override async UniTask Initialize(CancellationToken cancellationToken)
        {
            await base.Initialize(cancellationToken);

            View.RestartButtonClicked += OnRestartClicked;
        }

        public override void Dispose()
        {
            View.RestartButtonClicked -= OnRestartClicked;

            base.Dispose();
        }

        private void OnRestartClicked()
        {
            Data.GameplaySceneController.RestartLevel();
        }
    }
}