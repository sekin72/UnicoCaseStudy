using System.Threading;
using Cysharp.Threading.Tasks;
using UnicoCaseStudy.Managers.Data.Storages;

namespace UnicoCaseStudy.Managers.Data.Syncers
{
    public interface ILocalSyncer<T> where T : class, IStorageContainer, new()
    {
        UniTask<T> Load(CancellationToken cancellationToken);

        void Save(T data);

        void UpdateAccountId(string accountId);
    }
}