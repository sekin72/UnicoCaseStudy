using System;
using UnicoCaseStudy.Managers.Pool;
using UnicoCaseStudy.UI.Popups;

namespace UnicoCaseStudy.Gameplay.UI.Popups.Pause
{
    public class PausePopupData : PopupData
    {
        public readonly Action OnRestartButtonClicked;
        public readonly Action OnMMButtonClicked;
        public readonly Action OnClosed;

        public PausePopupData(Action onRestartButtonClicked, Action mmButtonClicked, Action onClosed)
            : base(PoolKeys.PausePopup)
        {
            OnRestartButtonClicked = onRestartButtonClicked;
            OnMMButtonClicked = mmButtonClicked;
            OnClosed = onClosed;
        }
    }
}