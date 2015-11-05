using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using RediSql.SqlClrComponents.Common;
using SqlClrDeclarations.Attributes;

namespace RediSql.SqlClrComponents
{
    public static class RedisqlGlobalServerFunctions
    {
        [SqlInstallerScriptGeneratorExportedFunction("GetServerInfo", "redisql")]
        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false, FillRowMethodName = "GetInfo_RowFiller", TableDefinition = "KeyName nvarchar(512), Value nvarchar(max)")]
        public static IEnumerable GetInfo(string host,
                                            [SqlParameter(DefaultValue = "6379")]int port,
                                            [SqlParameter(DefaultValue = typeof(DBNull))]string password,
                                            [SqlParameter(DefaultValue = typeof(DBNull))]int? dbId)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                return redis.GetInfo();
            }
        }

        public static void GetInfo_RowFiller(object item, out SqlString title, out SqlString value)
        {
            var settingRow = (KeyValuePair<string, string>)item;
            title = settingRow.Key;
            value = settingRow.Value;
        }

        [SqlInstallerScriptGeneratorExportedProcedure("SaveChanges", "redisql")]
        [SqlProcedure]
        public static void Save(string host,
                                            [SqlParameter(DefaultValue = "6379")]int port,
                                            [SqlParameter(DefaultValue = typeof(DBNull))]string password,
                                            [SqlParameter(DefaultValue = typeof(DBNull))]int? dbId,
                                            [SqlParameter(DefaultValue = true)] bool isBackground)

        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                if (isBackground)
                {
                    redis.BackgroundSave();
                }
                else
                {
                    redis.Save();
                }
            }
        }

        [SqlInstallerScriptGeneratorExportedProcedure("Flush", "redisql")]
        [SqlProcedure]
        public static void Flush(string host,
                                            [SqlParameter(DefaultValue = "6379")]int port,
                                            [SqlParameter(DefaultValue = typeof(DBNull))]string password,
                                            [SqlParameter(DefaultValue = typeof(DBNull))]int? dbId)

        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                if (dbId != null)
                {
                    redis.FlushDb();
                }
                else
                {
                    redis.FlushAll();
                }
            }
        }

        [SqlInstallerScriptGeneratorExportedFunction("GetLastSaved", "redisql")]
        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static DateTime GetLastSaved(string host,
                                            [SqlParameter(DefaultValue = "6379")]int port,
                                            [SqlParameter(DefaultValue = typeof(DBNull))]string password,
                                            [SqlParameter(DefaultValue = typeof(DBNull))]int? dbId)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                return redis.LastSave;
            }
        }
    }
}
