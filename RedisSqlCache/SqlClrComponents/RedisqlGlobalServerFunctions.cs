using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using RediSql.SqlClrComponents.Common;

namespace RediSql.SqlClrComponents
{
    public static class RedisqlGlobalServerFunctions
    {
        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false, FillRowMethodName = "GetInfo_RowFiller")]
        public static IEnumerable GetInfo(string host, int port, string password, int? dbId)
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

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static void Save(string host, int port, string password, int? dbId, bool isBackground)

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

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static void Flush(string host, int port, string password, int? dbId)

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

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static DateTime GetLastSaved(string host, int port, string password, int? dbId)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                return redis.LastSave;
            }
        }
    }
}
