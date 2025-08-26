using System.Threading;
using Cysharp.Threading.Tasks;

namespace UnicoCaseStudy.MVC
{
    public interface IController : ILifecycle
    {
        Data Data { get; }
        View View { get; }

        UniTask InitializeController(Data data, View view, CancellationToken cancellationToken);

        UniTask ActivateController(CancellationToken cancellationToken);

        void DeactivateController();

        void DisposeController();
    }
}