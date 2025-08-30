using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace UnicoCaseStudy.UI.Components
{
    public class SafeArea : MonoBehaviour
    {
        public static EdgeInsets MinimumPadding = new(0, 0, 0, 0);
        public static EdgeInsets MaximumPadding = new(2147483647, 2147483647, 0, 2147483647);

        public RectTransform RectTransform;

        public void Initialize()
        {
            UpdateSafeArea();
        }

        private static Rect GetAdjustSafeArea(Vector2 referenceResolution)
        {
            var safeArea = Screen.safeArea;

            safeArea.xMin = Mathf.Clamp(safeArea.xMin, MinimumPadding.Left, MaximumPadding.Left);
            safeArea.xMax = referenceResolution.x - Mathf.Clamp(
                referenceResolution.x - safeArea.xMax, MinimumPadding.Right, MaximumPadding.Right
            );

            safeArea.yMin = Mathf.Clamp(safeArea.yMin, MinimumPadding.Bottom, MaximumPadding.Bottom);
            safeArea.yMax = referenceResolution.y - Mathf.Clamp(
                referenceResolution.y - safeArea.yMax, MinimumPadding.Top, MaximumPadding.Top
            );

            return safeArea;
        }

        [Button("UpdateSafeArea")]
        public void UpdateSafeArea()
        {
            var currentResolution = Screen.currentResolution;
            var referenceResolution = new Vector2(currentResolution.width, currentResolution.height);
            var safeArea = GetAdjustSafeArea(referenceResolution);

            var minAnchor = safeArea.position;
            var maxAnchor = minAnchor + safeArea.size;

            minAnchor.x /= referenceResolution.x;
            minAnchor.y /= referenceResolution.y;
            maxAnchor.x /= referenceResolution.x;
            maxAnchor.y /= referenceResolution.y;

            RectTransform.anchorMin = minAnchor;

            RectTransform.anchorMax = maxAnchor;

            LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);
        }
    }
}