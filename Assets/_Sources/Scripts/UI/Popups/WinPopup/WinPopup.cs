using System.Threading;
using Cysharp.Threading.Tasks;

namespace UnicoCaseStudy.UI.Popups.Win
{
    public class WinPopup : Popup<WinPopupData, WinPopupView>
    {
        public override async UniTask Initialize(CancellationToken cancellationToken)
        {
            await base.Initialize(cancellationToken);

            View.BackToMainMenuButtonClicked += OnBackToMainClicked;
            View.CloseButtonClicked += OnBackToMainClicked;
        }

        public override void Dispose()
        {
            View.BackToMainMenuButtonClicked -= OnBackToMainClicked;
            View.CloseButtonClicked -= OnBackToMainClicked;

            base.Dispose();
        }

        private void OnBackToMainClicked()
        {
            Data.GameplaySceneController.ReturnToMainScene();
        }
    }
}