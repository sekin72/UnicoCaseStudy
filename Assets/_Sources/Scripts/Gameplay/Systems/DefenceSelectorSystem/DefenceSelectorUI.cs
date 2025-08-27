using deVoid.Utils;
using GameClient.GameData;
using UnicoCaseStudy.Gameplay.Signal;
using UnicoCaseStudy.UI.Components;
using UnityEngine;
using UnityEngine.UI;

namespace UnicoCaseStudy.Gameplay.Systems
{
    public class DefenceSelectorUI : MonoBehaviour
    {
        public DefenderConfig DefenderConfig { get; private set; }

        [SerializeField] private Image _image;
        [SerializeField] private CFText _remainingText;
        [SerializeField] private CFButton _button;

        private int _remainingCount;

        private Color _onColor;
        private Color _offColor;

        private bool _isActive;

        public void Initialize(DefenderConfig defenderConfig, int remainingCount)
        {
            DefenderConfig = defenderConfig;
            _remainingCount = remainingCount;

            _image.sprite = defenderConfig.Sprite;

            _onColor = _remainingText.TextFields[0].color;
            _offColor = Color.gray;
            _remainingText.Text = _remainingCount.ToString();

            _isActive = true;

            _button.onClick.AddListener(OnReleased);
        }

        public void Dispose()
        {
            _button.onClick.AddListener(OnReleased);
        }

        private void OnReleased()
        {
            if (!_isActive)
            {
                return;
            }

            Signals.Get<DefenceItemSelectedSignal>().Dispatch(this);
        }

        public void OnDefenderPlaced()
        {
            _remainingCount--;

            _remainingText.Text = _remainingCount.ToString();
            if (_remainingCount <= 0)
            {
                _isActive = false;
                SetCostImageColor(_isActive);
            }
        }

        private void SetCostImageColor(bool isActive)
        {
            _remainingText.SetColor(isActive ? _onColor : _offColor);
        }
    }
}
