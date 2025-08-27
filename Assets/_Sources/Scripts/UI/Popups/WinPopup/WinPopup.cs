using System.Threading;
using Cysharp.Threading.Tasks;
using UnicoCaseStudy.Managers.Gameplay;

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
            AppManager.GetManager<GameplayManager>().ReturnToMainScene();
        }
    }
}