﻿using System;
using System.Collections;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using RediSql.Common;
using RediSql.SqlClrComponents.Common;

namespace RediSql.SqlClrComponents
{
    public static class RedisqlKeysManipulationFunctions
    {
        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static bool IsKeyExists(string host, int port, string password, int? dbId, string key)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                return redis.ContainsKey(key);
            }
        }

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static bool Rename(string host, int port, string password, int? dbId, string key, string keyNewName)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                return redis.Rename(key, keyNewName);
            }
        }

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static bool SetRelativeExpiration(string host, int port, string password, int? dbId, string key,
          TimeSpan expiration)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                return redis.Expire(key, (int)expiration.TotalSeconds);
            }
        }

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static bool SetExactExpiration(string host, int port, string password, int? dbId, string key,
            DateTime expiration)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                return redis.ExpireAt(key, (int)DateTimeUtils.ToUnixTime(expiration));
            }
        }

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static int? GetKeyTTL(string host, int port, string password, int? dbId, string key)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                var ttl = redis.TimeToLive(key);
                return ttl >= 0 ? ttl : (int?)null;
            }
        }

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static bool DeleteKey(string host, int port, string password, int? dbId, string key)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                return redis.Remove(key);
            }
        }

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false, FillRowMethodName = "GetKeys_RowFiller")
        ]
        public static IEnumerable GetKeys(string host, int port, string password, int? dbId, string filter)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                return redis.GetKeys(filter);
            }
        }

        public static void GetKeys_RowFiller(object item, out SqlString keyName)
        {
            keyName = (string)item;
        }
    }
}