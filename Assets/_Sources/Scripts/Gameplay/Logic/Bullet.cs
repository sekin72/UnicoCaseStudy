using System.Threading;
using Cysharp.Threading.Tasks;
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

        private float _speed = 5;
        private bool _moving;
        private bool _reachedEnemy;
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

            _reachedEnemy = false;
            _moving = false;
            _disposed = false;

            MakeBulletAnimation().Forget();
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _reachedEnemy = false;
            _moving = false;
            _disposed = true;
            if (_attackCTS != null)
            {
                _attackCTS.Cancel();
                _attackCTS.Dispose();
                _attackCTS = null;
            }

            _poolManager.SafeReleaseObject(_bulletPoolKey, gameObject);
        }

        private async UniTask MakeBulletAnimation()
        {
            _moving = true;
            _reachedEnemy = false;
            _enemy.TakeDamageReal(_defenderConfig.Damage);

            transform.position = _defender.transform.position;

            await UniTask.WaitUntil(() => _reachedEnemy, cancellationToken: _attackCTS.Token);

            _enemy.TakeDamageEffective(_defenderConfig.Damage);

            Dispose();

            _defender.OnBulletReached(this);
        }

        private void Update()
        {
            if (!_moving || _disposed)
            {
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, _enemy.transform.position, _speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _enemy.transform.position) <= 0.1f)
            {
                _reachedEnemy = true;
                _moving = false;
            }
        }
    }
}
