using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using deVoid.Utils;
using GameClient.GameData;
using UnityEngine;

namespace UnicoCaseStudy.Gameplay.Logic
{
    public class Defender : BoardItem
    {
        private DefenderConfig _defenderConfig;
        private List<Vector2Int> _targetIndexes = new();
        private List<Enemy> _targetsInReach = new();

        private bool _canAttack = false;
        private CancellationTokenSource _attackCooldownCTS;
        private CancellationTokenSource _makeAttackCTS;

        public void Initialize(CharacterConfig config, GameplayTile attachedGameplayTile, GameObject idleVFX)
        {
            _defenderConfig = config as DefenderConfig;

            _sprite = config.Sprite;
            _spriteRenderer.sprite = _sprite;

            IdleVFX = idleVFX;
            IdleVFX.transform.SetParent(_idleVFXParent);
            IdleVFX.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));

            SetAttachedGameplayTile(attachedGameplayTile);

            SetTargetIndexes();
            _targetsInReach.Clear();

            Signals.Get<EnemyChangedTileSignal>().AddListener(OnEnemyChangedTile);
            Signals.Get<EnemyDiedSignal>().AddListener(OnEnemyDied);

            if (_attackCooldownCTS != null)
            {
                _attackCooldownCTS.Cancel();
                _attackCooldownCTS.Dispose();
                _attackCooldownCTS = null;
            }

            if (_makeAttackCTS != null)
            {
                _makeAttackCTS.Cancel();
                _makeAttackCTS.Dispose();
                _makeAttackCTS = null;
            }

            _attackCooldownCTS = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
            _makeAttackCTS = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());

            WaitAttackCooldown().Forget();
            MakeAttack().Forget();
        }

        public override void Dispose()
        {
            Signals.Get<EnemyChangedTileSignal>().RemoveListener(OnEnemyChangedTile);
            Signals.Get<EnemyDiedSignal>().RemoveListener(OnEnemyDied);

            if (_attackCooldownCTS != null)
            {
                _attackCooldownCTS.Cancel();
                _attackCooldownCTS.Dispose();
                _attackCooldownCTS = null;
            }

            if (_makeAttackCTS != null)
            {
                _makeAttackCTS.Cancel();
                _makeAttackCTS.Dispose();
                _makeAttackCTS = null;
            }

            base.Dispose();
        }

        private async UniTask WaitAttackCooldown()
        {
            IdleVFX.gameObject.SetActive(false);
            _canAttack = false;
            await UniTask.Delay(TimeSpan.FromSeconds(_defenderConfig.AttackCooldown), cancellationToken: _attackCooldownCTS.Token);
            _canAttack = true;
            IdleVFX.gameObject.SetActive(true);
        }

        private async UniTask MakeAttack()
        {
            await UniTask.WaitUntil(() => _canAttack && _targetsInReach.Any(x => x.RealHealth > 0), cancellationToken: _makeAttackCTS.Token);

            var enemy = _targetsInReach
                            .Where(x => x.RealHealth > 0)
                            .OrderBy(x => Vector3.Distance(transform.position, x.transform.position))
                            .FirstOrDefault();

            WaitAttackCooldown().Forget();
            MakeAttack().Forget();
            enemy.TakeDamage(_defenderConfig.Damage);
        }

        private void OnEnemyChangedTile(EnemyChangedTileSignalProperties signalProperties)
        {
            if (_targetsInReach.Contains(signalProperties.Enemy) && !_targetIndexes.Contains(signalProperties.NewTileIndex))
            {
                _targetsInReach.Remove(signalProperties.Enemy);
            }

            if (!_targetsInReach.Contains(signalProperties.Enemy) && _targetIndexes.Contains(signalProperties.NewTileIndex))
            {
                _targetsInReach.Add(signalProperties.Enemy);
            }
        }

        private void SetTargetIndexes()
        {
            _targetIndexes.Clear();

            switch (_defenderConfig.Direction)
            {
                case AttackDirection.Forward:
                    for (int i = 0; i <= _defenderConfig.Range; i++)
                    {
                        _targetIndexes.Add(AttachedGameplayTile.GameplayIndex + (Vector2Int.up * i));
                    }

                    break;
                case AttackDirection.EveryDirection:
                    for (int i = 0; i <= _defenderConfig.Range; i++)
                    {
                        _targetIndexes.Add(AttachedGameplayTile.GameplayIndex + (Vector2Int.left * i));
                        _targetIndexes.Add(AttachedGameplayTile.GameplayIndex + (Vector2Int.right * i));
                        _targetIndexes.Add(AttachedGameplayTile.GameplayIndex + (Vector2Int.up * i));
                    }

                    break;
                default:
                    break;
            }
        }

        private void OnEnemyDied(Enemy enemy)
        {
            if (_targetsInReach.Contains(enemy))
            {
                _targetsInReach.Remove(enemy);
            }
        }
    }
}
