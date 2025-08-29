using System.Threading;
using Cysharp.Threading.Tasks;
using UnicoCaseStudy.Gameplay.Logic;
using UnicoCaseStudy.Managers.Pool;
using UnityEngine;

namespace UnicoCaseStudy.Gameplay.Systems
{
    [CreateAssetMenu(fileName = "EnvironmentCreatorSystem", menuName = "UnicoCaseStudy/Systems/EnvironmentCreatorSystem", order = 1)]
    public sealed class EnvironmentCreatorSystem : GameSystem
    {
        public Tile[,] GroundTileArray => _view.GroundTileArray;
        public Tile[,] GameplayTileArray => _view.GameplayTileArray;

        private EnvironmentCreatorSystemView _view;

        private PoolManager _poolManager;

        public override async UniTask Initialize(GameSession gameSession, CancellationToken cancellationToken)
        {
            await base.Initialize(gameSession, cancellationToken);

            _poolManager = AppManager.GetManager<PoolManager>();

            _view = _poolManager.GetGameObject(PoolKeys.EnvironmentCreatorSystemView).GetComponent<EnvironmentCreatorSystemView>();
            _view.Initialize(Session.GameSettings);

            UnityEngine.Random.InitState(Session.GameSessionSaveStorage.LevelRandomSeed);
            await _view.CreateGroundTiles(cancellationToken);
            _view.CreateGameplayTiles();
            _view.FinalBoardAdjustments();
        }

        public override UniTask Activate(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public override void Deactivate()
        {
        }

        public override void Dispose()
        {
            _view.Dispose();
            _poolManager.SafeReleaseObject(PoolKeys.EnvironmentCreatorSystemView, _view.gameObject);
        }

        public Tile GetTile(Vector2Int index)
        {
            return _view.GetTile(index);
        }
    }
}
