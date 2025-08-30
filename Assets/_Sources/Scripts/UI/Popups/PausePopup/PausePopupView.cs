using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnicoCaseStudy.Managers.Vibration;
using UnicoCaseStudy.UI.Components;
using UnicoCaseStudy.UI.Popups;
using UnityEngine;
using UnityEngine.UI;

namespace UnicoCaseStudy.Gameplay.UI.Popups.Pause
{
    public class PausePopupView : PopupView
    {
        [SerializeField] protected CFToggleButton SoundButton;
        [SerializeField] protected Slider VolumeSlider;
        [SerializeField] protected GameObject SliderObject;
        [SerializeField] protected CFToggleButton VibrationButton;
        [SerializeField] protected GameObject VibrationObject;

        [SerializeField] protected Button CloseButton;
        [SerializeField] protected CFButton RestartButton;
        [SerializeField] protected CFButton MMButton;

        public event Action<bool> SoundToggled;

        public event Action<float> SoundVolumeChanged;

        public event Action<bool> VibrationToggled;

        public event Action CloseButtonClicked;

        public event Action RestartButtonClicked;

        public event Action MMButtonClicked;

        public override async UniTask Initialize(CancellationToken cancellationToken)
        {
            await base.Initialize(cancellationToken);

            SoundButton.OnValueChanged.AddListener(isOn => SoundToggled?.Invoke(isOn));
            VolumeSlider.onValueChanged.AddListener(value => SoundVolumeChanged?.Invoke(value));
            VibrationButton.OnValueChanged.AddListener(isOn => VibrationToggled?.Invoke(isOn));

            CloseButton.onClick.AddListener(() => CloseButtonClicked?.Invoke());
            RestartButton.onClick.AddListener(() => RestartButtonClicked?.Invoke());
            MMButton.onClick.AddListener(() => MMButtonClicked?.Invoke());
        }

        public override void Dispose()
        {
            SoundButton.OnValueChanged.RemoveAllListeners();
            VibrationButton.OnValueChanged.RemoveAllListeners();
            VolumeSlider.onValueChanged.RemoveAllListeners();
            CloseButton.onClick.RemoveAllListeners();
            RestartButton.onClick.RemoveAllListeners();
            MMButton.onClick.RemoveAllListeners();

            base.Dispose();
        }

        public void SetSound(bool isActive)
        {
            SoundButton.IsOn = isActive;
            SliderObject.SetActive(isActive);
        }

        public void SetVolume(float value)
        {
            VolumeSlider.value = value;
        }

        public void SetVibration(bool isActive)
        {
            if (VibrationButton != null)
            {
                VibrationButton.IsOn = isActive;
            }
        }
    }
}