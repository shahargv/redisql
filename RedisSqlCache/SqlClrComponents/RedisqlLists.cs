using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Server;
using RediSql.SqlClrComponents.Common;

namespace RediSql.SqlClrComponents
{
    public static class RedisqlLists
    {
        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false, FillRowMethodName = "GetListItems_RowFiller")]
        public static IEnumerable GetListItems(string host, int port, string password, int? dbId, string key, int start = 0, int end = -1)
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

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static string GetListItemAtIndex(string host, int port, string password, int? dbId, string key, int index)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                return Encoding.UTF8.GetString(redis.ListIndex(key, index));
            }
        }

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static string LeftPop(string host, int port, string password, int? dbId, string key)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                return Encoding.UTF8.GetString(redis.LeftPop(key));
            }
        }

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static string RightPop(string host, int port, string password, int? dbId, string key)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                return Encoding.UTF8.GetString(redis.RightPop(key));
            }
        }

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static void AddToList(string host, int port, string password, int? dbId, string key, string value, bool addToEnd, TimeSpan? expiration)
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
                    redis.Expire(key, (int) expiration.Value.TotalSeconds);
                }
            }
        }

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = false)]
        public static int GetListLength(string host, int port, string password, int? dbId, string key)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                return redis.ListLength(key);
            }
        }


    }
}
