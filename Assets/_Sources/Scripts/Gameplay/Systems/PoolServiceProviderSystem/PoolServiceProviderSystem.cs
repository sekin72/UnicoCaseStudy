using System.Threading;
using Cysharp.Threading.Tasks;
using UnicoCaseStudy.Managers.Pool;
using UnicoCaseStudy.MVC;
using UnityEngine;

namespace UnicoCaseStudy.Gameplay.Systems.ViewSpawner
{
    [CreateAssetMenu(fileName = "PoolServiceProviderSystem", menuName = "UnicoCaseStudy/Systems/PoolServiceProviderSystem", order = 2)]
    public sealed class PoolServiceProviderSystem : GameSystem
    {
        private PoolManager _poolManager;

        public override async UniTask Initialize(GameSession gameSession, CancellationToken cancellationToken)
        {
            await base.Initialize(gameSession, cancellationToken);

            _poolManager = AppManager.GetManager<PoolManager>();
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
        }

        public T Get<T>(PoolKeys poolKey) where T : View
        {
            var view = _poolManager.GetGameObject(poolKey).GetComponent<T>();

            view.transform.position = Vector3.zero;
            view.transform.rotation = Quaternion.identity;
            view.transform.localScale = Vector3.one;

            return view;
        }

        public void Release<T>(PoolKeys poolKey, T t) where T : View
        {
            _poolManager.SafeReleaseObject(poolKey, t.gameObject);
        }
    }
}