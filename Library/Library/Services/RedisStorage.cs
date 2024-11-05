using StackExchange.Redis;

namespace Library
{
    public class RedisStorage : IStorage
    {
        private readonly IDatabase db;
        private string _host = "localhost";

        public RedisStorage() {
            IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(_host);
            db = connectionMultiplexer.GetDatabase();
        }

        public string Load(string key)
        {
            return db.StringGet(key);
        }

        public void Store(string key, string value)
        {
            db.StringSet(key, value);
        }

        public bool IsExistsInSet(string setKey, string value)
        {
            return db.SetContains(setKey, value);
        }

        public void StoreToSet(string setKey, string value)
        {
            db.SetAdd(setKey, value);
        }
    }
}