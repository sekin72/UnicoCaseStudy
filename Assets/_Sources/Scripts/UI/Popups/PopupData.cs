using System;
using UnicoCaseStudy.Managers.Pool;
using UnicoCaseStudy.MVC;

namespace UnicoCaseStudy.UI.Popups
{
    public abstract class PopupData : Data
    {
        public readonly PoolKeys PoolKey;
        public readonly bool ShowDarkinator;
        public Action OnCloseClicked { get; private set; }
        public Action CloseCall { get; private set; }

        protected PopupData(PoolKeys poolKey,
            bool showDarkinator = true,
            Action onCloseClicked = null)
        {
            PoolKey = poolKey;
            ShowDarkinator = showDarkinator;
            OnCloseClicked = onCloseClicked;
        }

        public void AttachCloseCall(Action closeCall)
        {
            CloseCall = closeCall;
        }
    }
}