using System;
using UnicoCaseStudy.Managers.Pool;
using UnicoCaseStudy.UI.Popups;

namespace UnicoCaseStudy.Gameplay.UI.Popups.Pause
{
    public class PausePopupData : PopupData
    {
        public readonly Action OnSaveButtonClicked;
        public readonly Action OnLoadButtonClicked;
        public readonly Action OnMMButtonClicked;

        public PausePopupData(Action onSaveButtonClicked, Action onLoadButtonClicked, Action mmButtonClicked, Action onAfterClosed)
            : base(PoolKeys.PausePopup, true, onAfterClosed)
        {
            OnSaveButtonClicked = onSaveButtonClicked;
            OnLoadButtonClicked = onLoadButtonClicked;
            OnMMButtonClicked = mmButtonClicked;
        }
    }
}