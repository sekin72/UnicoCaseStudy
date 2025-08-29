using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using deVoid.Utils;
using GameClient.GameData;
using UnicoCaseStudy.Gameplay.Signal;
using UnicoCaseStudy.UI.Components;
using UnicoCaseStudy.Utilities.MonoBehaviourUtilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnicoCaseStudy.Gameplay.Systems
{
    public class DefenceSelectorUI : PlayerInputBase
    {
        public DefenderConfig DefenderConfig { get; private set; }

        [SerializeField] private Image _image;
        [SerializeField] private CFText _remainingText;
        [SerializeField] private CFButton _button;

        private int _remainingCount;

        private Color _onColor;
        private Color _offColor;

        private bool _canBePlaced => !_isInCooldown && _isActive;
        private bool _isInCooldown;
        private bool _isActive;

        private CancellationTokenSource _placementCooldownCTS;

        public void Initialize(DefenderConfig defenderConfig, int remainingCount)
        {
            DefenderConfig = defenderConfig;
            _remainingCount = remainingCount;

            _image.sprite = defenderConfig.Sprite;
            _image.raycastTarget = true;

            _onColor = _remainingText.TextFields[0].color;
            _offColor = Color.gray;
            _remainingText.Text = _remainingCount.ToString();

            _isInCooldown = false;
            _isActive = true;
        }

        public void Dispose()
        {
            if (_placementCooldownCTS != null)
            {
                _placementCooldownCTS.Cancel();
                _placementCooldownCTS.Dispose();
                _placementCooldownCTS = null;
            }
        }

        public void OnDefenderPlaced()
        {
            _remainingCount--;

            _isInCooldown = true;
            _remainingText.Text = _remainingCount.ToString();
            if (_remainingCount <= 0)
            {
                _isActive = false;
            }

            SetCostImageColor();
            _placementCooldownCTS = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
            WaitForPlacementCooldown().Forget();
        }

        private async UniTask WaitForPlacementCooldown()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(DefenderConfig.PlacementCooldown));

            _isInCooldown = false;
            SetCostImageColor();
        }

        private void SetCostImageColor()
        {
            _remainingText.SetColor(_canBePlaced ? _onColor : _offColor);
        }

        protected override void OnMultitouchOtherFingersDragged(PointerEventData pointerEventData)
        {
        }

        protected override void OnObjectDragged(PointerEventData pointerEventData)
        {
        }

        protected override void OnObjectSelected(PointerEventData pointerEventData)
        {
            if (!_canBePlaced)
            {
                return;
            }

            Signals.Get<DefenceItemSelectedSignal>().Dispatch(this, pointerEventData);
            _button.OnPointerDown(pointerEventData);
            _image.raycastTarget = false;
        }

        protected override void OnObjectReleased(PointerEventData pointerEventData)
        {
            if (!_canBePlaced)
            {
                return;
            }

            Signals.Get<DefenceItemReleasedSignal>().Dispatch(pointerEventData);
            _button.OnPointerUp(pointerEventData);
            _image.raycastTarget = true;
        }

        protected override void OnFingerChanged(PointerEventData oldFingerData, int newFingerID)
        {
        }

        protected override void InitOnActivate()
        {
            MultiTouchSupport = false;
        }
    }
}
