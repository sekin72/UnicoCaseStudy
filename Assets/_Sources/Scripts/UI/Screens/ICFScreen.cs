using UnicoCaseStudy.MVC;
using UnicoCaseStudy.UI.Components;

namespace UnicoCaseStudy.UI.Screens
{
    public interface ICFScreen
    {
        new CFScreenView View { get; }
        new CFScreenData Data { get; }
        public Darkinator Darkinator { get; }
        public SafeArea SafeArea { get; }
    }
}