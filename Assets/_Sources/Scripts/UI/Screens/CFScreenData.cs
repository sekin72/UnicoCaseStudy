using UnicoCaseStudy.MVC;
using UnicoCaseStudy.UI.Popups.Helpers;

namespace UnicoCaseStudy.UI.Screens
{
    public abstract class CFScreenData : Data
    {
        public readonly CFScreenAnchors ScreenAnchor;

        protected CFScreenData(CFScreenAnchors screenAnchor)
        {
            ScreenAnchor = screenAnchor;
        }
    }
}