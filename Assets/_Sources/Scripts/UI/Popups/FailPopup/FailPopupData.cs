using UnicoCaseStudy.Managers.Pool;
using UnicoCaseStudy.SceneControllers;

namespace UnicoCaseStudy.UI.Popups.Fail
{
    public class FailPopupData : PopupData
    {
        public readonly GameplaySceneController GameplaySceneController;

        public FailPopupData(GameplaySceneController gameplaySceneController) : base(PoolKeys.FailPopup)
        {
            GameplaySceneController = gameplaySceneController;
        }
    }
}