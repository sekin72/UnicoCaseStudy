namespace UnicoCaseStudy.Managers.Data.Storages
{
    public interface IStorageContainer
    {
        T Get<T>() where T : class, IStorage, new();

        void Set<T>(T data) where T : class, IStorage, new();
    }
}