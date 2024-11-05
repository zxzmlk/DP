namespace Library
{
    public interface IStorage
    {
        void Store(string key, string value);
        void Store(string segment, string key, string value);
        string Load(string segment, string key);
        string GetSegmentId(string key);
        void StoreToSet(string setKey, string value);
        void StoreToSet(string segment, string setKey, string value);
        bool IsExistsInSet(string setKey, string value);
    }
}