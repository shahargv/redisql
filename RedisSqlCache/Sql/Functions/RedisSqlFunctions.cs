using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Server;

namespace RedisSqlCache.Sql.Functions
{
    public static class RedisSqlFunctions
    {

        private static Redis GetConnection(string host, int port, string password = null, int? dbId = null)
        {
            Redis redis = new Redis(host, port);
            if (password != null)
                redis.Password = password;
            if (dbId != null)
                redis.Db = dbId.Value;
            return redis;
        }
        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static bool IsKeyExists(string host, int port, string password, int? dbId, string key)
        {
            var redis = GetConnection(host, port, password, dbId);
            return redis.ContainsKey(key);
        }

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static void SetStringValue(string host, int port, string password, int? dbId, string key, string value, TimeSpan? expiration)
        {
            var redis = GetConnection(host, port, password, dbId);
            redis.Set(key, value);
            if (expiration != null)
                redis.Expire(key, (int)expiration.Value.TotalSeconds);
        }

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static string GetSetStringValue(string host, int port, string password, int? dbId, string key, string value, TimeSpan? expiration)
        {
            var redis = GetConnection(host, port, password, dbId);
            string result = redis.GetSet(key, value);
            if (expiration != null)
                redis.Expire(key, (int)expiration.Value.TotalSeconds);
            return result;
        }

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static string GetStringValue(string host, int port, string password, int? dbId, string key)
        {
            var redis = GetConnection(host, port, password, dbId);
            return redis.GetString(key);
        }

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static bool DeleteKey(string host, int port, string password, int? dbId, string key)
        {
            var redis = GetConnection(host, port, password, dbId);
            return redis.Remove(key);
        }
    }
}
