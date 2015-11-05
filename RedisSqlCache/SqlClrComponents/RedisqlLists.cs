using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Server;
using RediSql.SqlClrComponents.Common;
using SqlClrDeclarations.Attributes;

namespace RediSql.SqlClrComponents
{
    public static class RedisqlLists
    {
        [SqlInstallerScriptGeneratorExportedFunction("GetListItems", "redisql")]
        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false, FillRowMethodName = "GetListItems_RowFiller", TableDefinition = "Value nvarchar(max)")]
        public static IEnumerable GetListItems(string host,
                                        [SqlParameter(DefaultValue = "6379")]int port,
                                        [SqlParameter(DefaultValue = typeof(DBNull))]string password,
                                        [SqlParameter(DefaultValue = typeof(DBNull))]int? dbId,
                                        string key,
                                        [SqlParameter(DefaultValue = 0)]int start,
                                        [SqlParameter(DefaultValue = -1)]int end)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                return redis.ListRange(key, start, end);
            }
        }

        public static void GetListItems_RowFiller(object item, out SqlString value)
        {
            byte[] txtEncoded = (byte[])item;
            value = Encoding.UTF8.GetString(txtEncoded);
        }

        [SqlInstallerScriptGeneratorExportedFunction("GetListItemsAtIndex", "redisql")]
        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static string GetListItemAtIndex(string host,
                                                [SqlParameter(DefaultValue = "6379")]int port,
                                                [SqlParameter(DefaultValue = typeof(DBNull))]string password,
                                                [SqlParameter(DefaultValue = typeof(DBNull))]int? dbId,
                                                string key,
                                                int index)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                return Encoding.UTF8.GetString(redis.ListIndex(key, index));
            }
        }

        [SqlInstallerScriptGeneratorExportedFunction("LeftPop", "redisql")]
        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static string LeftPop(string host,
                                                [SqlParameter(DefaultValue = "6379")]int port,
                                                [SqlParameter(DefaultValue = typeof(DBNull))]string password,
                                                [SqlParameter(DefaultValue = typeof(DBNull))]int? dbId,
                                                string key)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                return Encoding.UTF8.GetString(redis.LeftPop(key));
            }
        }

        [SqlInstallerScriptGeneratorExportedFunction("RightPop", "redisql")]
        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static string RightPop(string host,
                                                [SqlParameter(DefaultValue = "6379")]int port,
                                                [SqlParameter(DefaultValue = typeof(DBNull))]string password,
                                                [SqlParameter(DefaultValue = typeof(DBNull))]int? dbId,
                                                string key)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                return Encoding.UTF8.GetString(redis.RightPop(key));
            }
        }

        [SqlInstallerScriptGeneratorExportedProcedure("AddToList", "redisql")]
        [SqlProcedure]
        public static void AddToList(string host,
                                                [SqlParameter(DefaultValue = "6379")]int port,
                                                [SqlParameter(DefaultValue = typeof(DBNull))]string password,
                                                [SqlParameter(DefaultValue = typeof(DBNull))]int? dbId,
                                                string key,
                                                string value,
                                                [SqlParameter(DefaultValue = true)]bool addToEnd,
                                                [SqlParameter(DefaultValue = typeof(DBNull))] TimeSpan? expiration)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                if (addToEnd)
                {
                    redis.RightPush(key, value);
                }
                else
                {
                    redis.LeftPush(key, value);
                }
                if (expiration != null)
                {
                    redis.Expire(key, (int)expiration.Value.TotalSeconds);
                }
            }
        }

        [SqlInstallerScriptGeneratorExportedFunction("GetListLength", "redisql")]
        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static int GetListLength(string host,
                                                [SqlParameter(DefaultValue = "6379")]int port,
                                                [SqlParameter(DefaultValue = typeof(DBNull))]string password,
                                                [SqlParameter(DefaultValue = typeof(DBNull))]int? dbId,
                                                string key)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                return redis.ListLength(key);
            }
        }


    }
}
