using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameClient.GameData;
using UnicoCaseStudy.Gameplay.Logic;
using UnicoCaseStudy.Managers.Asset;
using UnicoCaseStudy.Managers.Pool;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UnicoCaseStudy.Gameplay.Systems.EnvironmentCreatorSystem
{
    [CreateAssetMenu(fileName = "EnvironmentCreatorSystem", menuName = "UnicoCaseStudy/Systems/EnvironmentCreatorSystem", order = 1)]
    public sealed class EnvironmentCreatorSystem : GameSystem
    {
        private EnvironmentCreatorSystemView _view;

        private PoolManager _poolManager;

        public List<Tile> Path { get; private set; }

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
