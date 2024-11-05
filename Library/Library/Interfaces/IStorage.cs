namespace Library
{
    public interface IStorage
    {
        void Store(string key, string value);
        string Load(string key);
        void StoreToSet(string setKey, string value);
        bool IsExistsInSet(string setKey, string value);
    }
}