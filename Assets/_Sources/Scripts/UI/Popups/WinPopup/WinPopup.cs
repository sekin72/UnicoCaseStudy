using System.Threading;
using Cysharp.Threading.Tasks;

namespace UnicoCaseStudy.UI.Popups.Win
{
    public class WinPopup : Popup<WinPopupData, WinPopupView>
    {
        public override async UniTask Initialize(CancellationToken cancellationToken)
        {
            await base.Initialize(cancellationToken);

            View.NextLevelButtonClicked += OnNextLevelClicked;
            View.CloseButtonClicked += OnBackToMainClicked;
        }

        public override void Dispose()
        {
            View.NextLevelButtonClicked -= OnNextLevelClicked;
            View.CloseButtonClicked -= OnBackToMainClicked;

            base.Dispose();
        }

        private void OnNextLevelClicked()
        {
            OnCloseClicked();
            Data.OnMMButtonClicked?.Invoke();
        }

        private void OnBackToMainClicked()
        {
            OnCloseClicked();
            Data.OnMMButtonClicked?.Invoke();
        }
    }
}