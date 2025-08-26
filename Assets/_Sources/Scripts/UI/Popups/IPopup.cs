using UnicoCaseStudy.MVC;

namespace UnicoCaseStudy.UI.Popups
{
    public interface IPopup
    {
        new PopupData Data { get; }
        new PopupView View { get; }
        public string UniqueName { get; }

        void TapOutside();

        void GoBack();
    }
}