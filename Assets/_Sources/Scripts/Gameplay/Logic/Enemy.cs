using System.Threading;
using Cysharp.Threading.Tasks;
using deVoid.Utils;
using DG.Tweening;
using GameClient.GameData;
using UnicoCaseStudy.Gameplay.Logic;
using UnicoCaseStudy.Gameplay.Signal;
using UnicoCaseStudy.Managers.Sound;
using UnicoCaseStudy.Managers.Vibration;
using UnicoCaseStudy.Utilities.MonoBehaviourUtilities;
using UnityEngine;

namespace UnicoCaseStudy
{
    public class Enemy : BoardItem
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private AnimationEventTriggerDetector _animationEventTriggerDetector;

        private SoundManager _soundManager;
        private VibrationManager _vibrationManager;

        public int EffectiveHealth { get; private set; }
        public int RealHealth { get; private set; }

        public Vector2Int CurrentTile { get; private set; }
        public Vector2Int TargetTile { get; private set; }
        private EnemyConfig _enemyConfig;

        private Tween _moveTween;
        private CancellationTokenSource _moveTweenCTS;

        public void Initialize(CharacterConfig config, Vector2Int currentTile, Vector2Int targetTile)
        {
            _soundManager = AppManager.GetManager<SoundManager>();
            _vibrationManager = AppManager.GetManager<VibrationManager>();

            _sprite = config.Sprite;
            _spriteRenderer.sprite = _sprite;

            CurrentTile = currentTile;
            TargetTile = targetTile;

            _enemyConfig = config as EnemyConfig;
            UpdateSortingOrder();

            _animationEventTriggerDetector.AnimEventOccured += OnAnimEventOccured;
            _moveTweenCTS = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());

            RealHealth = _enemyConfig.Health;
            EffectiveHealth = RealHealth;

            _animator.enabled = true;
            _animator.runtimeAnimatorController = _enemyConfig.AnimatorController;
        }

        public void Deactivate()
        {
            if (_moveTweenCTS == null)
            {
                return;
            }

            _animator.enabled = false;
            _animationEventTriggerDetector.AnimEventOccured -= OnAnimEventOccured;
            _moveTweenCTS.Cancel();
            _moveTweenCTS.Dispose();
            _moveTweenCTS = null;
            _moveTween?.Kill();
            _moveTween = null;
        }

        protected override void UpdateSortingOrder()
        {
            _spriteRenderer.sortingOrder = SortingOrderConstants.Enemies;

            //foreach (var renderer in IdleVFX.GetComponentsInChildren<ParticleSystemRenderer>())
            //{
            //    renderer.sortingOrder = SortingOrderConstants.EnemyVFX;
            //}
        }

        public async UniTask StartMovement()
        {
            await MoveUntilNextTile();
            await MoveUntilTileCenter();

            if (CurrentTile == TargetTile)
            {
                Signals.Get<LevelFinishedSignal>().Dispatch(false);
                return;
            }

            StartMovement().Forget();
        }

        private async UniTask MoveUntilNextTile()
        {
            var target = transform.position + (Vector3.down * 0.5f);
            var duration = Vector2.Distance(transform.position, target) / _enemyConfig.Speed;
            _moveTween?.Kill();
            _moveTween = transform.DOMove(target, duration).SetEase(Ease.Linear);
            _moveTween.Play();

            await _moveTween.ToUniTask(TweenCancelBehaviour.KillAndCancelAwait, cancellationToken: _moveTweenCTS.Token);

            Signals.Get<EnemyChangedTileSignal>().Dispatch(new EnemyChangedTileSignalProperties(this, CurrentTile, CurrentTile + Vector2Int.down));

            CurrentTile += Vector2Int.down;
        }

        private async UniTask MoveUntilTileCenter()
        {
            var target = transform.position + (Vector3.down * 0.5f);
            var duration = Vector2.Distance(transform.position, target) / _enemyConfig.Speed;
            _moveTween?.Kill();
            _moveTween = transform.DOMove(target, duration).SetEase(Ease.Linear);
            _moveTween.Play();

            await _moveTween.ToUniTask(TweenCancelBehaviour.KillAndCancelAwait, cancellationToken: _moveTweenCTS.Token);
        }

        public void TakeDamageEffective(int damage)
        {
            EffectiveHealth -= damage;

            _moveTween?.Pause();
            if (RealHealth <= 0)
            {
                _vibrationManager.Vibrate(VibrationType.MediumImpact);
                _animator.SetTrigger(AnimationConstants.Death);
                _soundManager.PlayOneShot(SoundKeys.Positive);
            }
            else
            {
                _vibrationManager.Vibrate(VibrationType.LightImpact);
                _animator.SetTrigger(AnimationConstants.Damage);
                _soundManager.PlayOneShot(SoundKeys.Attack);
            }
        }

        public void TakeDamageReal(int damage)
        {
            RealHealth -= damage;
        }

        private void OnAnimEventOccured(string obj)
        {
            if (obj.Equals("DamageTaken"))
            {
                _moveTween?.Play();
            }
            else if (obj.Equals("Death"))
            {
                Signals.Get<EnemyDiedSignal>().Dispatch(this);
            }
        }
    }
}
