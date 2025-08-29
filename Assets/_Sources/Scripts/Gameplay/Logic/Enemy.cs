using System.Threading;
using Cysharp.Threading.Tasks;
using deVoid.Utils;
using DG.Tweening;
using GameClient.GameData;
using UnicoCaseStudy.Gameplay.Logic;
using UnicoCaseStudy.Gameplay.Signal;
using UnityEngine;

namespace UnicoCaseStudy
{
    public class Enemy : BoardItem
    {
        public Vector2Int CurrentTile { get; private set; }
        public Vector2Int TargetTile { get; private set; }
        private EnemyConfig _enemyConfig;

        private Tween _moveTween;
        private CancellationTokenSource _moveTweenCTS;

        public void Initialize(CharacterConfig config, Vector2Int currentTile, Vector2Int targetTile)
        {
            _sprite = config.Sprite;
            _spriteRenderer.sprite = _sprite;

            CurrentTile = currentTile;
            TargetTile = targetTile;

            _enemyConfig = config as EnemyConfig;
            UpdateSortingOrder();

            _moveTweenCTS = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
        }

        public void Deactivate()
        {
            if (_moveTweenCTS == null)
            {
                return;
            }

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

            await _moveTween.ToUniTask(TweenCancelBehaviour.KillAndCancelAwait, cancellationToken: _moveTweenCTS.Token);
        }
    }
}
