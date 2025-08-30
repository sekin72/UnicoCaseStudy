using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameClient.GameData;
using UnicoCaseStudy.Gameplay.Logic;
using UnicoCaseStudy.Managers.Pool;
using UnityEngine;

namespace UnicoCaseStudy
{
    public class Bullet : MonoBehaviour
    {
        private PoolManager _poolManager;
        private PoolKeys _bulletPoolKey;
        private PoolKeys _impactPoolKey;

        private Defender _defender;
        private DefenderConfig _defenderConfig;
        private Enemy _enemy;

        private GameObject _impact;

        private CancellationTokenSource _attackCTS;
        private Tween _moveTween;

        private bool _disposed;

        public void Initialize(
            PoolKeys bulletKey,
            PoolKeys impactPoolKey,
            Defender defender,
            DefenderConfig defenderConfig,
            Enemy enemy)
        {
            _poolManager = AppManager.GetManager<PoolManager>();

            _bulletPoolKey = bulletKey;
            _impactPoolKey = impactPoolKey;
            _defender = defender;
            _defenderConfig = defenderConfig;
            _enemy = enemy;

            _attackCTS = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());

            _disposed = false;

            MakeBulletAnimation().Forget();
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            if (_attackCTS != null)
            {
                _attackCTS.Cancel();
                _attackCTS.Dispose();
                _attackCTS = null;
            }

            _moveTween?.Kill();

            if (_impact != null)
            {
                _poolManager.SafeReleaseObject(_impactPoolKey, _impact.gameObject);
            }

            _poolManager.SafeReleaseObject(_bulletPoolKey, gameObject);
        }

        private async UniTask MakeBulletAnimation()
        {
            _enemy.TakeDamageReal(_defenderConfig.Damage);

            transform.position = _defender.transform.position;

            _moveTween?.Kill();
            _moveTween = transform.DOMove(_enemy.transform.position, 0.5f);

            await _moveTween.ToUniTask(cancellationToken: _attackCTS.Token);

            _enemy.TakeDamageEffective(_defenderConfig.Damage);

            _impact = _poolManager.GetGameObject(_impactPoolKey);
            _impact.transform.position = _enemy.transform.position;

            await UniTask.Delay(TimeSpan.FromTicks(8), cancellationToken: _attackCTS.Token);

            Dispose();
        }
    }
}
