using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameClient.GameData;
using UnicoCaseStudy.Managers.Pool;
using UnityEngine;

namespace UnicoCaseStudy.Gameplay.Systems
{
    [CreateAssetMenu(fileName = "EnemyControllerSystem", menuName = "UnicoCaseStudy/Systems/EnemyControllerSystem", order = 6)]
    public class EnemyControllerSystem : GameSystem
    {
        private PoolManager _poolManager;

        private EnvironmentCreatorSystem _environmentCreatorSystem;

        private Queue<EnemyConfig> _waveQueue;
        private List<Enemy> _placedEnemies;
        private float _delayBetweenSpawns;

        private CancellationTokenSource _delayCTS;

        private int _gameplayX;
        private int _gameplayTopY;

        public override async UniTask Activate(CancellationToken cancellationToken)
        {
            _poolManager = AppManager.GetManager<PoolManager>();

            _environmentCreatorSystem = Session.GetSystem<EnvironmentCreatorSystem>();

            _waveQueue = new Queue<EnemyConfig>();
            _placedEnemies = new List<Enemy>();

            _delayBetweenSpawns = Session.LevelConfig.DelayBetweenSpawns;
            foreach (var enemyConfig in Session.LevelConfig.EnemyWave)
            {
                _waveQueue.Enqueue(enemyConfig);
            }

            _gameplayX = (Session.GameSettings.TotalWidth - Session.GameSettings.GameplayWidth) / 2;
            _gameplayTopY = (Session.GameSettings.TotalHeight + Session.GameSettings.GameplayHeight) / 2;

            await UniTask.Delay(TimeSpan.FromSeconds(_delayBetweenSpawns), cancellationToken: cancellationToken);

            LoadEnemies().Forget();
        }

        public override void Deactivate()
        {
            if (_delayCTS != null)
            {
                _delayCTS.Cancel();
                _delayCTS.Dispose();
                _delayCTS = null;
            }

            foreach (var enemy in _placedEnemies)
            {
                enemy.Deactivate();
                _poolManager.SafeReleaseObject(PoolKeys.Enemy, enemy.gameObject);
            }
        }

        public override void Dispose()
        {
        }

        private async UniTask LoadEnemies()
        {
            var enemyDataHolder = _waveQueue.Dequeue();

            var gameplayAreaStartX = (Session.GameSettings.TotalWidth - Session.GameSettings.GameplayWidth) / 2;
            var gameplayAreaEndY = (Session.GameSettings.TotalHeight + Session.GameSettings.GameplayHeight) / 2;

            var gameplayX = UnityEngine.Random.Range(0, Session.GameSettings.GameplayWidth);
            var gameplayY = Session.GameSettings.GameplayHeight;

            var startTile = _environmentCreatorSystem.GroundTileArray[gameplayAreaStartX + gameplayX, gameplayAreaEndY];

            var enemyBoardItem = _poolManager.GetGameObject(PoolKeys.Enemy).GetComponent<Enemy>();
            enemyBoardItem.Initialize(enemyDataHolder, new Vector2Int(gameplayX, gameplayY), new Vector2Int(gameplayX, -1));
            enemyBoardItem.transform.position = startTile.TileObject.transform.position;
            _placedEnemies.Add(enemyBoardItem);

            enemyBoardItem.StartMovement().Forget();

            _delayCTS = new CancellationTokenSource();
            await UniTask.Delay(TimeSpan.FromSeconds(_delayBetweenSpawns), cancellationToken: _delayCTS.Token);

            _delayCTS.Cancel();
            _delayCTS.Dispose();
            _delayCTS = null;

            if (_waveQueue.Count == 0)
            {
                return;
            }

            LoadEnemies().Forget();
        }
    }
}
