using System.Threading;
using Cysharp.Threading.Tasks;
using deVoid.Utils;
using UnicoCaseStudy.Managers.Pool;
using UnicoCaseStudy.MVC;
using UnicoCaseStudy.Signal;
using UnicoCaseStudy.UI.Components;
using UnicoCaseStudy.UI.Popups;
using UnityEngine;

namespace UnicoCaseStudy.Managers.UI
{
    public sealed class PopupManager : Manager
    {
        private const string OpeningLockBinName = "OpeningPopupLockBin";

        private IPopup _displayedPopup;
        private IController _displayedPopupController => _displayedPopup as IController;

        private PoolManager _poolManager;

        public UIContainer Container { get; private set; }

        protected override UniTask Initialize(CancellationToken disposeToken)
        {
            _poolManager = AppManager.GetManager<PoolManager>();

            Signals.Get<UIContainerChangedSignal>().AddListener(OnUIContainerCreated);

            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            Close();

            Signals.Get<UIContainerChangedSignal>().RemoveListener(OnUIContainerCreated);

            base.Dispose();
        }

        private void OnUIContainerCreated(UIContainerChangedSignalProperties evt)
        {
            SetContextCanvas(evt.UIContainer);
        }

        private void SetContextCanvas(UIContainer container)
        {
            Container = container;
            Container.CurrentScreen.Darkinator.Initialize(this, OnTapOutside);
        }

        private void OnTapOutside()
        {
            _displayedPopup.TapOutside();
        }

        public async UniTask Open<TP, TD, TV>(
            TD data,
            CancellationToken cancellationToken)
            where TP : Popup<TD, TV>, new()
            where TD : PopupData
            where TV : PopupView
        {
            CFButton.DisableInput(OpeningLockBinName);

            try
            {
                var viewPrefab = _poolManager.GetGameObject(data.PoolKey);
                var popupView = viewPrefab.GetComponent<TV>();
                var popupController = new TP();

                popupView.transform.SetParent(Container.CurrentScreen.SafeArea.transform, false);

                await popupController.InitializeController(data, popupView, cancellationToken);

                popupView.Root.rotation = Quaternion.identity;
                popupView.Root.localScale = Vector3.one;
                ((RectTransform)popupView.Root).anchorMin = Vector2.zero;
                ((RectTransform)popupView.Root).anchorMax = Vector2.one;
                ((RectTransform)popupView.Root).offsetMin = Vector2.zero;
                ((RectTransform)popupView.Root).offsetMax = Vector2.zero;

                if (data.ShowDarkinator)
                {
                    Container.CurrentScreen.Darkinator.AttachBlackScreen(popupController.UniqueName, popupView.transform);
                }

                _displayedPopup = popupController;

                await popupController.ActivateController(cancellationToken);
            }
            finally
            {
                CFButton.EnableInput(OpeningLockBinName);
            }
        }

        public void Close()
        {
            if (_displayedPopup == null)
            {
                return;
            }

            if (_displayedPopup.Data.ShowDarkinator)
            {
                Container.CurrentScreen.Darkinator.DetachBlackScreen(_displayedPopup.UniqueName);
            }

            _displayedPopupController.DeactivateController();
            _displayedPopupController.DisposeController();
            _poolManager.SafeReleaseObject(_displayedPopup.Data.PoolKey, _displayedPopup.View.Root.gameObject);

            _displayedPopup = null;
        }
    }
}