using deVoid.Utils;
using UnicoCaseStudy.UI.Components;

namespace UnicoCaseStudy.Signal
{
    public class UIContainerChangedSignal : ASignal<UIContainerChangedSignalProperties>
    {
    }

    public readonly struct UIContainerChangedSignalProperties
    {
        public readonly UIContainer UIContainer;

        public UIContainerChangedSignalProperties(UIContainer uiContainer)
        {
            UIContainer = uiContainer;
        }
    }
}
