using System.Threading;
using Cysharp.Threading.Tasks;
using UnicoCaseStudy.UI.Screens.Default;
using UnicoCaseStudy.UI.Screens.Main;

namespace UnicoCaseStudy.UI.Screens
{
    public class MainMenuScreenGroup : ScreenGroup
    {
        protected override async UniTask SetScreens(CancellationToken cancellationToken)
        {
            Screens = new ICFScreen[ScreenViews.Length];

            var mainScreen = new MainScreen();
            var mainScreenData = new MainScreenData();
            var mainScreenView = ScreenViews[0] as MainScreenView;
            Screens[0] = mainScreen;

            await mainScreen.InitializeController(mainScreenData, mainScreenView, cancellationToken);

            for (var i = 1; i < ScreenViews.Length; i++)
            {
                var screen = new DefaultScreen();
                var screenData = new DefaultScreenData();
                var screenView = ScreenViews[i] as DefaultScreenView;
                Screens[i] = screen;

                await screen.InitializeController(screenData, screenView, cancellationToken);
            }
        }
    }
}