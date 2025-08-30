using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using deVoid.Utils;
using DG.Tweening;
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

        [SerializeField] private GameObject _infoPanel;
        [SerializeField] private CFText _rangeText;
        [SerializeField] private CFText _damageText;
        [SerializeField] private CFText _directionText;

        [SerializeField] private Image _fillerImage;

        private int _remainingCount;

        private Color _onColor;
        private Color _offColor;

        private bool _canBePlaced => !_isInCooldown && _isActive;
        private bool _isInCooldown;
        private bool _isActive;

        private CancellationTokenSource _placementCooldownCTS;
        private Tween _fillTween;

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
            _button.interactable = true;

            SetCostImageColor();

            _rangeText.Text = $"Range: {defenderConfig.Range}";
            _damageText.Text = $"Damage: {defenderConfig.Damage}";
            _directionText.Text = $"Direction: {defenderConfig.Direction.ToString()}";
            _infoPanel.SetActive(false);

            _fillTween?.Kill();
            _fillerImage.gameObject.SetActive(false);
        }

        public void Dispose()
        {
            _fillTween?.Kill();

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
                _button.interactable = false;
            }

            SetCostImageColor();
            _placementCooldownCTS = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
            WaitForPlacementCooldown().Forget();
        }

        private async UniTask WaitForPlacementCooldown()
        {
            if (_isActive)
            {
                _fillerImage.gameObject.SetActive(true);
                _fillerImage.fillAmount = 1;
                _fillTween?.Kill();
                _fillTween = _fillerImage.DOFillAmount(0, DefenderConfig.PlacementCooldown);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(DefenderConfig.PlacementCooldown), cancellationToken: _placementCooldownCTS.Token);

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
            _infoPanel.SetActive(true);
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
            _infoPanel.SetActive(false);
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
