using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnicoCaseStudy.Managers.Gameplay;

namespace UnicoCaseStudy.UI.Popups.Fail
{
    public class FailPopup : Popup<FailPopupData, FailPopupView>
    {
        public override async UniTask Initialize(CancellationToken cancellationToken)
        {
            await base.Initialize(cancellationToken);

            View.RestartButtonClicked += OnRestartClicked;
            View.MMButtonClicked += OnMMClicked;
        }

        public override void Dispose()
        {
            View.RestartButtonClicked -= OnRestartClicked;
            View.MMButtonClicked -= OnMMClicked;

            base.Dispose();
        }

        private void OnRestartClicked()
        {
            ClosePopup();
            Data.OnRestartButtonClicked?.Invoke();
        }

        private void OnMMClicked()
        {
            ClosePopup();
            Data.OnMMButtonClicked?.Invoke();
        }
    }
}