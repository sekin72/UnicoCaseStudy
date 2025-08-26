using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UnicoCaseStudy
{
    public interface ILifecycle
    {
       UniTask Initialize(CancellationToken cancellationToken);
       UniTask Activate(CancellationToken cancellationToken);
       void Deactivate();
       void Dispose();
    }
}