using System.Threading;
using Cysharp.Threading.Tasks;
using UnicoCaseStudy.UI.Screens;
using UnityEngine;
using UnityEngine.UI;

namespace UnicoCaseStudy.UI.Components
{
    public class UIContainer : MonoBehaviour
    {
        public ICFScreen CurrentScreen => ScreenGroup.CurrentScreen;

        [SerializeField] private ScreenGroup ScreenGroup;

        public Canvas Canvas;
        public CanvasScaler CanvasScaler;

        public async UniTask Initialize(CancellationToken cancellationToken)
        {
            await ScreenGroup.Initialize(cancellationToken);
        }

        public async UniTask Activate(CancellationToken cancellationToken)
        {
            gameObject.SetActive(true);

            CanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            CanvasScaler.matchWidthOrHeight = 0.5f;

            CanvasScaler.referenceResolution = new Vector2(828, 1792);

            await ScreenGroup.Activate(cancellationToken);

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)Canvas.transform);
        }

        public void Deactivate()
        {
            ScreenGroup.Deactivate();
            ScreenGroup.Dispose();
            gameObject.SetActive(false);
        }
    }
}