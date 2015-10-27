using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.SqlServer.Server;
using RediSql.SqlClrComponents.Common;

namespace RediSql.SqlClrComponents
{
    public static class RedisqlRowsets
    {
        [SqlFunction(DataAccess = DataAccessKind.Read, IsDeterministic = false)]
        public static void StoreQueryResultsData(string host, int port, string password, int? dbId, string key,
            string query, TimeSpan? expiration)
        {
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                using (SqlConnection connection = new SqlConnection("context connection=true"))
                {
                    connection.Open();
                    using (SqlCommand queryCommand = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader rowsetReader = queryCommand.ExecuteReader())
                        {
                            while (rowsetReader.Read())
                            {
                                XElement el = new XElement("item");
                                for (int index = 0; index < rowsetReader.FieldCount; index++)
                                {
                                    el.Add(new XElement(rowsetReader.GetName(index), rowsetReader.GetValue(index)));
                                }
                                RedisqlLists.AddToList(host, port, password, dbId, key, el.ToString(), true, null);
                            }

                        }
                        if (expiration != null)
                        {
                            redis.Expire(key, (int)expiration.Value.TotalSeconds);
                        }
                    }
                }
            }
        }
    }
}
