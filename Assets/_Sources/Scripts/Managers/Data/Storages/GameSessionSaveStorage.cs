namespace UnicoCaseStudy.Managers.Data.Storages
{
    public class GameSessionSaveStorage : IStorage
    {
        public bool GameplayFinished = false;
        public int CurrentLevel = 0;
        public int LevelRandomSeed = 0;
        public LevelConfig LevelConfig;
    }
}