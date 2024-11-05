using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Implementations;
using System;
using System.Collections.Generic;

namespace Library
{
    public class RedisStorage : IStorage
    {
        private readonly IDatabase db;
        private readonly Dictionary<string, IDatabase> connections;
        private string _host = "localhost";

        public RedisStorage() {
            IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(_host);
            db = connectionMultiplexer.GetDatabase();

            connections = new Dictionary<string, IDatabase>
            {
                {
                    Const.SEGMENT_RUS,
                    ConnectionMultiplexer.Connect(
                Environment.GetEnvironmentVariable("DB_RUS", EnvironmentVariableTarget.User)
            ).GetDatabase()
                },
                {
                    Const.SEGMENT_EU,
                    ConnectionMultiplexer.Connect(
                Environment.GetEnvironmentVariable("DB_EU", EnvironmentVariableTarget.User)
            ).GetDatabase()
                },
                {
                    Const.SEGMENT_OTHER,
                    ConnectionMultiplexer.Connect(
                Environment.GetEnvironmentVariable("DB_OTHER", EnvironmentVariableTarget.User)
            ).GetDatabase()
                }
            };
        }

        public void Store(string key, string value)
        {
            db.StringSet(key, value);
        }
        public void Store(string segment, string key, string value)
        {
            GetConnection(segment).StringSet(key, value);
        }

        public bool IsExistsInSet(string setKey, string value)
        {
            // Проверяем в каждом сегменте
            foreach (KeyValuePair<string, IDatabase> connection in connections)
            {
                if (connection.Value.SetContains(setKey, value))
                {
                    return true;
                }
            }

            return false;
        }

        public void StoreToSet(string setKey, string value)
        {
            db.SetAdd(setKey, value);
        }
        public void StoreToSet(string segment, string setKey, string value)
        {
            GetConnection(segment).SetAdd(setKey, value);
        }
        private IDatabase GetConnection(string segment)
        {
            return connections[segment];
        }

        public string GetSegmentId(string key)
        {
            return db.StringGet(Const.SegmentKey + key);
        }

        public string Load(string segment, string key)
        {
            return GetConnection(segment).StringGet(key);
        }
    }
}