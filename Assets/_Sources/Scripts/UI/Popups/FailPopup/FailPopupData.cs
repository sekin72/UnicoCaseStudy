using System;
using UnicoCaseStudy.Managers.Pool;
using UnicoCaseStudy.SceneControllers;

namespace UnicoCaseStudy.UI.Popups.Fail
{
    public class FailPopupData : PopupData
    {
        public readonly Action OnRestartButtonClicked;
        public readonly Action OnMMButtonClicked;
        public FailPopupData(Action onRestartButtonClicked, Action mmButtonClicked) : base(PoolKeys.FailPopup)
        {
            OnRestartButtonClicked = onRestartButtonClicked;
            OnMMButtonClicked = mmButtonClicked;
        }
    }
}