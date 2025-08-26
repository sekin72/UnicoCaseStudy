using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnicoCaseStudy.MVC;
using UnicoCaseStudy.UI.Components;
using UnicoCaseStudy.UI.Screens.Default;
using UnicoCaseStudy.Utilities.Extensions;
using UnityEngine;

namespace UnicoCaseStudy.UI.Screens
{
    public class ScreenGroup : View
    {
        public ICFScreen CurrentScreen { get; private set; }
        public IController CurrentScreenController => CurrentScreen as IController;

        [SerializeField] private CFButton[] Buttons;
        [SerializeField] protected CFScreenView[] ScreenViews;
        [SerializeField] private int _mainScreenIndex = 0;

        protected ICFScreen[] Screens;
        private int _currentScreenIndex;

        private float _intervalX;

        protected CancellationTokenSource DisposeTokenSource;

        public override async UniTask Initialize(CancellationToken cancellationToken)
        {
            _intervalX = Screen.width;

            DisposeTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            await SetScreens(cancellationToken);

            CurrentScreen = Screens[_mainScreenIndex];
            ForceMoveByButton(_mainScreenIndex);

            for (var i = 0; i < Buttons.Length; i++)
            {
                var index = i;
                Buttons[i].onClick.AddListener(() => ForceMoveByButton(index));
            }
        }

        public override async UniTask Activate(CancellationToken cancellationToken)
        {
            var tasks = new List<UniTask>();
            for (var i = 0; i < ScreenViews.Length; i++)
            {
                tasks.Add(AsController(Screens[i]).ActivateController(cancellationToken));
            }

            await UniTask.WhenAll(tasks);
        }

        public override void Deactivate()
        {
            for (var i = 0; i < ScreenViews.Length; i++)
            {
                AsController(Screens[i]).DeactivateController();
            }
        }

        public override void Dispose()
        {
            Screens.DoForAll(x => AsController(x).DisposeController());
            foreach (var screenButton in Buttons)
            {
                screenButton.onClick.RemoveAllListeners();
            }

            DisposeTokenSource?.Cancel();
            DisposeTokenSource?.Dispose();
        }

        protected virtual async UniTask SetScreens(CancellationToken cancellationToken)
        {
            Screens = new ICFScreen[ScreenViews.Length];
            for (var i = 0; i < ScreenViews.Length; i++)
            {
                var screen = new DefaultScreen();
                var screenData = new DefaultScreenData();
                var screenView = ScreenViews[i] as DefaultScreenView;
                Screens[i] = screen;

                await screen.InitializeController(screenData, screenView, cancellationToken);
            }
        }

        private void ForceMoveByButton(int index)
        {
            if (index == _currentScreenIndex)
            {
                return;
            }

            ChangeScreen(index);
        }

        private void ChangeScreen(int index)
        {
            CurrentScreenController.DeactivateController();

            _currentScreenIndex = index;
            CurrentScreen = Screens[index];

            var position = transform.position;
            position.x = (-index * _intervalX) + (_intervalX / 2);
            transform.position = position;

            CurrentScreenController.ActivateController(this.GetCancellationTokenOnDestroy());
        }

        private IController AsController(ICFScreen screen)
        {
            return screen as IController;
        }
    }
}