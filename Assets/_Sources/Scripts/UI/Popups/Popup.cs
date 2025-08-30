using System.Threading;
using Cysharp.Threading.Tasks;
using UnicoCaseStudy.Managers.UI;
using UnicoCaseStudy.MVC;

namespace UnicoCaseStudy.UI.Popups
{
    public abstract class Popup<TD, TV> : Controller<TD, TV>, IPopup
        where TD : PopupData
        where TV : PopupView
    {
        private PopupManager _popupManager;
        public string UniqueName { get; private set; }

        #region Lifecycle

        PopupData IPopup.Data => Data;
        PopupView IPopup.View => View;

        public override UniTask Initialize(CancellationToken cancellationToken)
        {
            _popupManager = AppManager.GetManager<PopupManager>();
            UniqueName = $"{GetType().Name}";

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
        }

        public override void Dispose()
        {
        }

        #endregion Lifecycle

        public void TapOutside()
        {
            OnTapOutside();
        }

        public void GoBack()
        {
            OnTapOutside();
        }

        protected virtual void OnTapOutside()
        {
            ClosePopup();
        }

        protected virtual void OnCloseClicked()
        {
            ClosePopup();
        }

        protected void ClosePopup()
        {
            _popupManager.Close();
        }
    }
}