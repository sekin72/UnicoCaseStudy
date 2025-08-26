using System.Threading;
using Cysharp.Threading.Tasks;

namespace UnicoCaseStudy.Managers
{
    public interface IManager
    {
        public UniTask StartAsync(CancellationToken cancellation);
        void Dispose();
    }
}