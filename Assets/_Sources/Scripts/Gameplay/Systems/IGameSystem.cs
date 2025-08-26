using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UnicoCaseStudy.Gameplay.Systems
{
    public interface IGameSystem : ILifecycle
    {
        UniTask Initialize(GameSession gameSession, CancellationToken cancellationToken);
    }
}