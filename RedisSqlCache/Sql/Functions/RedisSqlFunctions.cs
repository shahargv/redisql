using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
using Microsoft.SqlServer.Server;
using RedisSqlCache.Common;

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
        public static void SetStringValue(string host, int port, string password, int? dbId, string key, string value,
            TimeSpan? expiration)
        {
            var redis = GetConnection(host, port, password, dbId);
            redis.Set(key, value);
            if (expiration != null)
                redis.Expire(key, (int)expiration.Value.TotalSeconds);
        }

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static bool SetStringValueIfNotExists(string host, int port, string password, int? dbId, string key,
            string value, TimeSpan? expiration)
        {
            var redis = GetConnection(host, port, password, dbId);
            bool wasKeyCreated = redis.SetNX(key, value);
            if (wasKeyCreated && expiration != null)
                return redis.Expire(key, (int)expiration.Value.TotalSeconds);
            return wasKeyCreated;
        }

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static bool Rename(string host, int port, string password, int? dbId, string key, string keyNewName)
        {
            var redis = GetConnection(host, port, password, dbId);
            return redis.Rename(key, keyNewName);
        }

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static bool SetRelativeExpiration(string host, int port, string password, int? dbId, string key,
            TimeSpan expiration)
        {
            var redis = GetConnection(host, port, password, dbId);
            return redis.Expire(key, (int)expiration.TotalSeconds);
        }

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static bool SetExactExpiration(string host, int port, string password, int? dbId, string key,
            DateTime expiration)
        {
            var redis = GetConnection(host, port, password, dbId);
            return redis.ExpireAt(key, (int)DateTimeUtils.ToUnixTime(expiration));
        }

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static int? GetKeyTTL(string host, int port, string password, int? dbId, string key)
        {
            var redis = GetConnection(host, port, password, dbId);
            var ttl = redis.TimeToLive(key);
            return ttl >= 0 ? ttl : (int?)null;
        }

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static void Save(string host, int port, string password, int? dbId, bool isBackground)

        {
            var redis = GetConnection(host, port, password, dbId);
            if (isBackground)
            {
                redis.BackgroundSave();
            }
            else
            {
                redis.Save();
            }
        }

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static void Flush(string host, int port, string password, int? dbId)

        {
            var redis = GetConnection(host, port, password, dbId);
            if (dbId != null)
            {
                redis.FlushDb();
            }
            else
            {
                redis.FlushAll();
            }
        }

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static DateTime GetLastSaved(string host, int port, string password, int? dbId)
        {
            var redis = GetConnection(host, port, password, dbId);
            return redis.LastSave;
        }

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static string GetSetStringValue(string host, int port, string password, int? dbId, string key,
            string value, TimeSpan? expiration)
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

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false, FillRowMethodName = "GetKeys_RowFiller")]
        public static IEnumerable GetKeys(string host, int port, string password, int? dbId, string filter)
        {
            var redis = GetConnection(host, port, password, dbId);
            return redis.GetKeys(filter);
        }

        public static void GetKeys_RowFiller(object item, out SqlString keyName)
        {
            keyName = (string)item;
        }

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false, FillRowMethodName = "GetInfo_RowFiller")]
        public static IEnumerable GetInfo(string host, int port, string password, int? dbId)
        {
            var redis = GetConnection(host, port, password, dbId);
            return redis.GetInfo();
        }

        public static void GetInfo_RowFiller(object item, out SqlString title, out SqlString value)
        {
            var settingRow = (KeyValuePair<string, string>)item;
            title = settingRow.Key;
            value = settingRow.Value;
        }
    }
}
