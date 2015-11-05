using System;
using Microsoft.SqlServer.Server;
using RediSql.SqlClrComponents.Common;
using SqlClrDeclarations.Attributes;

namespace RediSql.SqlClrComponents
{
    public static class RedisqlStringValuesFunctions
    {
        [SqlInstallerScriptGeneratorExportedProcedure("SetStringValue", "redisql")]
        [SqlProcedure]
        public static void SetStringValue(string host,
                                                    [SqlParameter(DefaultValue = "6379")]int port,
                                                    [SqlParameter(DefaultValue = typeof(DBNull))]string password,
                                                    [SqlParameter(DefaultValue = typeof(DBNull))]int? dbId,
                                                    string key,
                                                    string value,
                                                    [SqlParameter(DefaultValue = typeof(DBNull))]TimeSpan? expiration)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                redis.Set(key, value);
                if (expiration != null)
                    redis.Expire(key, (int)expiration.Value.TotalSeconds);
            }
        }

        [SqlInstallerScriptGeneratorExportedFunction("SetStringValueIfNotExists", "redisql")]
        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static bool SetStringValueIfNotExists(string host,
                                                    [SqlParameter(DefaultValue = "6379")]int port,
                                                    [SqlParameter(DefaultValue = typeof(DBNull))]string password,
                                                    [SqlParameter(DefaultValue = typeof(DBNull))]int? dbId,
                                                    string key,
                                                    string value,
                                                    [SqlParameter(DefaultValue = typeof(DBNull))]TimeSpan? expiration)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                bool wasKeyCreated = redis.SetNX(key, value);
                if (wasKeyCreated && expiration != null)
                    return redis.Expire(key, (int)expiration.Value.TotalSeconds);
                return wasKeyCreated;
            }
        }

        [SqlInstallerScriptGeneratorExportedFunction("GetSetStringValue", "redisql")]
        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static string GetSetStringValue(string host,
                                                    [SqlParameter(DefaultValue = "6379")]int port,
                                                    [SqlParameter(DefaultValue = typeof(DBNull))]string password,
                                                    [SqlParameter(DefaultValue = typeof(DBNull))]int? dbId,
                                                    string key,
                                                    string value,
                                                    [SqlParameter(DefaultValue = typeof(DBNull))]TimeSpan? expiration)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                string result = redis.GetSet(key, value);
                if (expiration != null)
                    redis.Expire(key, (int)expiration.Value.TotalSeconds);
                return result;
            }
        }

        [SqlInstallerScriptGeneratorExportedFunction("GetStringValue", "redisql")]
        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static string GetStringValue(string host,
                                                    [SqlParameter(DefaultValue = "6379")]int port,
                                                    [SqlParameter(DefaultValue = typeof(DBNull))]string password,
                                                    [SqlParameter(DefaultValue = typeof(DBNull))]int? dbId,
                                                    string key)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                return redis.GetString(key);
            }
        }

    }
}
