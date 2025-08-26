using System.Threading;
using Cysharp.Threading.Tasks;
using UnicoCaseStudy.Gameplay.Systems;
using UnityEngine;

namespace UnicoCaseStudy.Gameplay
{
    public abstract class GameSystem : ScriptableObject, IGameSystem
    {
        protected GameSession Session;

        protected string LockBinKey;

        public virtual UniTask Initialize(GameSession gameSession, CancellationToken cancellationToken)
        {
            Session = gameSession;

            LockBinKey = GetType().ToString();

            return UniTask.CompletedTask;
        }

        public UniTask Initialize(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public abstract UniTask Activate(CancellationToken cancellationToken);

        public abstract void Deactivate();

        public abstract void Dispose();
    }
}