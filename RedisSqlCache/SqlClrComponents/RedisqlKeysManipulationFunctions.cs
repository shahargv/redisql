using System;
using System.Collections;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using RediSql.Common;
using RediSql.SqlClrComponents.Common;
using SqlClrDeclarations.Attributes;

namespace RediSql.SqlClrComponents
{
    public static class RedisqlKeysManipulationFunctions
    {
        [SqlInstallerScriptGeneratorExportedFunction("IsKeyExists", "redisql")]
        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static bool IsKeyExists(string host,
                                        [SqlParameter(DefaultValue = "6379")]int port,
                                        [SqlParameter(DefaultValue = typeof(DBNull))]string password,
                                        [SqlParameter(DefaultValue = typeof(DBNull))]int? dbId,
                                        string key)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                return redis.ContainsKey(key);
            }
        }

        [SqlInstallerScriptGeneratorExportedFunction("GetKeyType", "redisql")]
        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static string GetKeyType(string host,
                                        [SqlParameter(DefaultValue = "6379")]int port,
                                        [SqlParameter(DefaultValue = typeof(DBNull))]string password,
                                        [SqlParameter(DefaultValue = typeof(DBNull))]int? dbId,
                                        string key)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                var redisKeyType = redis.TypeOf(key);
                switch (redisKeyType)
                {
                    case Redis.KeyType.None:
                        return KeyType.NotExisting.ToString();
                    case Redis.KeyType.String:
                        return KeyType.String.ToString();
                    case Redis.KeyType.List:
                        if (RedisqlLists.GetListItemAtIndex(host, port, password, dbId, key, 0).Equals(RedisqlRowsets.RowsetMagic, StringComparison.OrdinalIgnoreCase))
                            return KeyType.Rowset.ToString();
                        return KeyType.List.ToString();
                    case Redis.KeyType.Set:
                        return KeyType.Set.ToString();
                    case Redis.KeyType.ZSet:
                        return KeyType.Set.ToString();
                    case Redis.KeyType.Hash:
                        return KeyType.Hash.ToString();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        [SqlInstallerScriptGeneratorExportedFunction("RenameKey", "redisql")]
        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static bool Rename(string host,
                                        [SqlParameter(DefaultValue = "6379")]int port,
                                        [SqlParameter(DefaultValue = typeof(DBNull))]string password,
                                        [SqlParameter(DefaultValue = typeof(DBNull))]int? dbId,
                                        string key,
                                        string keyNewName)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                return redis.Rename(key, keyNewName);
            }
        }

        [SqlInstallerScriptGeneratorExportedFunction("SetRelativeExpiration", "redisql")]
        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static bool SetRelativeExpiration(string host,
                                                    [SqlParameter(DefaultValue = "6379")]int port,
                                                    [SqlParameter(DefaultValue = typeof(DBNull))]string password,
                                                    [SqlParameter(DefaultValue = typeof(DBNull))]int? dbId,
                                                    string key,
                                                    TimeSpan expiration)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                return redis.Expire(key, (int)expiration.TotalSeconds);
            }
        }

        [SqlInstallerScriptGeneratorExportedFunction("SetExactExpiration", "redisql")]
        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static bool SetExactExpiration(string host,
                                                    [SqlParameter(DefaultValue = "6379")]int port,
                                                    [SqlParameter(DefaultValue = typeof(DBNull))]string password,
                                                    [SqlParameter(DefaultValue = typeof(DBNull))]int? dbId,
                                                    string key,
                                                    DateTime expiration)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                return redis.ExpireAt(key, (int)DateTimeUtils.ToUnixTime(expiration));
            }
        }

        [SqlInstallerScriptGeneratorExportedFunction("GetKeyTTL", "redisql")]
        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static int? GetKeyTTL(string host,
                                                    [SqlParameter(DefaultValue = "6379")]int port,
                                                    [SqlParameter(DefaultValue = typeof(DBNull))]string password,
                                                    [SqlParameter(DefaultValue = typeof(DBNull))]int? dbId,
                                                    string key)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                var ttl = redis.TimeToLive(key);
                return ttl >= 0 ? ttl : (int?)null;
            }
        }

        [SqlInstallerScriptGeneratorExportedFunction("DeleteKey", "redisql")]
        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static bool DeleteKey(string host,
                                        [SqlParameter(DefaultValue = "6379")]int port,
                                        [SqlParameter(DefaultValue = typeof(DBNull))]string password,
                                        [SqlParameter(DefaultValue = typeof(DBNull))]int? dbId,
                                        string key)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                return redis.Remove(key);
            }
        }

        [SqlInstallerScriptGeneratorExportedFunction("GetKeys", "redisql")]
        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false, FillRowMethodName = "GetKeys_RowFiller", TableDefinition = "KeyName nvarchar(512)")]
        public static IEnumerable GetKeys(string host,
                                            [SqlParameter(DefaultValue = "6379")]int port,
                                            [SqlParameter(DefaultValue = typeof(DBNull))]string password,
                                            [SqlParameter(DefaultValue = typeof(DBNull))]int? dbId,
                                            [SqlParameter(DefaultValue = "*")]string filter)
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
