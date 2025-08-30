using System;
using UnicoCaseStudy.Managers.Pool;

namespace UnicoCaseStudy.UI.Popups.Win
{
    public class WinPopupData : PopupData
    {
        public readonly Action OnMMButtonClicked;
        public WinPopupData(Action mmButtonClicked) : base(PoolKeys.WinPopup)
        {
            OnMMButtonClicked = mmButtonClicked;
        }
    }
}