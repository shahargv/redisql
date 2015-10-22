using System;
using Microsoft.SqlServer.Server;
using RediSql.SqlClrComponents.Common;

namespace RediSql.SqlClrComponents
{
    public static class RedisqlStringValuesFunctions
    {

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static void SetStringValue(string host, int port, string password, int? dbId, string key, string value,
            TimeSpan? expiration)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                redis.Set(key, value);
                if (expiration != null)
                    redis.Expire(key, (int)expiration.Value.TotalSeconds);
            }
        }

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static bool SetStringValueIfNotExists(string host, int port, string password, int? dbId, string key,
            string value, TimeSpan? expiration)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                bool wasKeyCreated = redis.SetNX(key, value);
                if (wasKeyCreated && expiration != null)
                    return redis.Expire(key, (int)expiration.Value.TotalSeconds);
                return wasKeyCreated;
            }
        }

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static string GetSetStringValue(string host, int port, string password, int? dbId, string key,
                 string value, TimeSpan? expiration)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                string result = redis.GetSet(key, value);
                if (expiration != null)
                    redis.Expire(key, (int)expiration.Value.TotalSeconds);
                return result;
            }
        }

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static string GetStringValue(string host, int port, string password, int? dbId, string key)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {

                return redis.GetString(key);
            }
        }

    }
}
