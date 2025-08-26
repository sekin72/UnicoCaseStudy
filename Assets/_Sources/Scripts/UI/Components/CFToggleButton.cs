using UnicoCaseStudy.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnicoCaseStudy.UI.Components
{
    [AddComponentMenu("UI/CF/CFToggleButton")]
    public class CFToggleButton : Button
    {
        public static readonly LockBin IsInputLocked = new();

        public GameObject OnState;
        public GameObject OffState;
        public bool IsOn;

        public Toggle.ToggleEvent OnValueChanged = new();

        protected CFToggleButton()
        {
        }

        protected override void Start()
        {
            base.Start();
            Refresh();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (IsInputLocked)
            {
                return;
            }

            base.OnPointerDown(eventData);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (IsInputLocked)
            {
                return;
            }

            base.OnPointerClick(eventData);
            ToggleState();
        }

        private void ToggleState()
        {
            IsOn = !IsOn;

            OnValueChanged.Invoke(IsOn);
            Refresh();
        }

        public void Refresh()
        {
            OnState?.SetActive(IsOn);
            OffState?.SetActive(!IsOn);
        }
    }
}