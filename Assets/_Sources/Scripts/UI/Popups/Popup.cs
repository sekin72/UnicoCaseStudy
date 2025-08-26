using System.Threading;
using Cysharp.Threading.Tasks;
using UnicoCaseStudy.MVC;

namespace UnicoCaseStudy.UI.Popups
{
    public abstract class Popup<TD, TV> : Controller<TD, TV>, IPopup
        where TD : PopupData
        where TV : PopupView
    {
        public string UniqueName { get; private set; }

        #region Lifecycle

        PopupData IPopup.Data => Data;
        PopupView IPopup.View => View;

        public override UniTask Initialize(CancellationToken cancellationToken)
        {
            UniqueName = $"{GetType().Name}";
            View.CloseButtonClicked += ClosePopup;

            return UniTask.CompletedTask;
        }

        public override UniTask Activate(CancellationToken cancellationToken)
        {
            View.SetVisible(true);
            return UniTask.CompletedTask;
        }

        public override void Deactivate()
        {
            View.SetVisible(false);
            View.CloseButtonClicked -= ClosePopup;
        }

        public override void Dispose()
        {
        }

        #endregion Lifecycle

        public void SetCloseButtonVisible(bool isVisible)
        {
            View.SetCloseButtonVisible(isVisible);
        }

        public virtual void TapOutside()
        {
            ClosePopup();
        }

        public virtual void GoBack()
        {
            ClosePopup();
        }

        protected void ClosePopup()
        {
            Data.OnCloseClicked?.Invoke();
            Data.CloseCall?.Invoke();
        }
    }
}