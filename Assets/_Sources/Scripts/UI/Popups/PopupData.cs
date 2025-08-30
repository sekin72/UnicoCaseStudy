using System;
using UnicoCaseStudy.Managers.Pool;
using UnicoCaseStudy.MVC;

namespace UnicoCaseStudy.UI.Popups
{
    public abstract class PopupData : Data
    {
        public readonly PoolKeys PoolKey;
        public readonly bool ShowDarkinator;

        protected PopupData(PoolKeys poolKey,
            bool showDarkinator = true)
        {
            PoolKey = poolKey;
            ShowDarkinator = showDarkinator;
        }
    }
}