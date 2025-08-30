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

        private Defender _defender;
        private DefenderConfig _defenderConfig;
        private Enemy _enemy;

        private CancellationTokenSource _attackCTS;
        private Tween _moveTween;

        private bool _disposed;

        public void Initialize(
            PoolKeys bulletKey,
            Defender defender,
            DefenderConfig defenderConfig,
            Enemy enemy)
        {
            _poolManager = AppManager.GetManager<PoolManager>();

            _bulletPoolKey = bulletKey;
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

            _poolManager.SafeReleaseObject(_bulletPoolKey, gameObject);
        }

        private async UniTask MakeBulletAnimation()
        {
            _enemy.TakeDamageReal(_defenderConfig.Damage);

            transform.position = _defender.transform.position;

            var duration = Vector3.Distance(_enemy.transform.position, transform.position) / 8f;
            _moveTween?.Kill();
            _moveTween = transform.DOMove(_enemy.transform.position, duration).SetEase(Ease.Linear);

            await _moveTween.ToUniTask(cancellationToken: _attackCTS.Token);

            _enemy.TakeDamageEffective(_defenderConfig.Damage);

            Dispose();
        }
    }
}
