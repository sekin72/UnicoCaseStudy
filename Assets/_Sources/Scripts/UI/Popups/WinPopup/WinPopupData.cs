using UnicoCaseStudy.Managers.Pool;
using UnicoCaseStudy.SceneControllers;

namespace UnicoCaseStudy.UI.Popups.Win
{
    public class WinPopupData : PopupData
    {
        public readonly GameplaySceneController GameplaySceneController;

        public WinPopupData(GameplaySceneController gameplaySceneController)
            : base(PoolKeys.WinPopup)
        {
            GameplaySceneController = gameplaySceneController;
        }
    }
}