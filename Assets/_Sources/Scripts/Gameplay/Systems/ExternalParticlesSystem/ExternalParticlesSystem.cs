using System.Threading;
using Cysharp.Threading.Tasks;
using deVoid.Utils;
using UnicoCaseStudy.Managers.Pool;
using UnicoCaseStudy.Signal;
using UnityEngine;

namespace UnicoCaseStudy.Gameplay.Systems.ExternalParticles
{
    [CreateAssetMenu(fileName = "ExternalParticlesSystem", menuName = "UnicoCaseStudy/Systems/ExternalParticlesSystem", order = 3)]
    public sealed class ExternalParticlesSystem : GameSystem
    {
        private ExternalParticlesSystemView _view;
        private PoolManager _poolManager;

        public override async UniTask Initialize(GameSession gameSession, CancellationToken cancellationToken)
        {
            await base.Initialize(gameSession, cancellationToken);

            _poolManager = AppManager.GetManager<PoolManager>();

            _view = _poolManager.GetGameObject(PoolKeys.ExternalParticlesSystemView).GetComponent<ExternalParticlesSystemView>();

            Signals.Get<AttachParticleSignal>().AddListener(OnAttachParticleSignal);
            Signals.Get<DetachParticleSignal>().AddListener(OnDetachParticleSignal);
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
            _poolManager.SafeReleaseObject(PoolKeys.ExternalParticlesSystemView, _view.gameObject);
        }

        private void OnAttachParticleSignal(AttachParticleSignalProperties properties)
        {
            _view.AttachGameObject(properties.PoolKey, properties.ParticleGameObject);
        }

        private void OnDetachParticleSignal(DetachParticleSignalProperties properties)
        {
            _view.DetachGameObject(properties.PoolKey, properties.ParticleGameObject);
        }
    }
}