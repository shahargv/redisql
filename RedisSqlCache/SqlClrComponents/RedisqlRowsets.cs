using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.SqlServer.Server;
using RediSql.SqlClrComponents.Common;
using SqlClrDeclarations.Attributes;

namespace RediSql.SqlClrComponents
{
    public static class RedisqlRowsets
    {
        [SqlInstallerScriptGeneratorExportedProcedure("StoreQueryResultsData", "redisql")]
        [SqlProcedure]
        public static void StoreQueryResultsData(string host,
                                                    [SqlParameter(DefaultValue = "6379")]int port,
                                                    [SqlParameter(DefaultValue = typeof(DBNull))]string password,
                                                    [SqlParameter(DefaultValue = typeof(DBNull))]int? dbId,
                                                    string key,
                                                    string query,
                                                    [SqlParameter(DefaultValue = typeof(DBNull))]TimeSpan? expiration,
                                                    [SqlParameter(DefaultValue = false)]bool replaceExisting)
        {
            var rowsXmls = ExecuteQueryAndGetResultList(query);
            using (var redis = RedisConnection.GetConnection(host, port, password, dbId))
            {
                if (redis.ContainsKey(key))
                {
                    if (replaceExisting)
                    {
                        redis.Remove(key);
                    }
                    else
                    {
                        throw new Exception("key with the same name already exists, and replace flag not enabled");
                    }
                }
                List<string> valuesToAdd = new List<string>(rowsXmls.Count + 1) { RowsetMagic };
                valuesToAdd.AddRange(rowsXmls);
                valuesToAdd.ForEach(val => RedisqlLists.AddToList(host, port, password, dbId, key, val, true, null));
                if (expiration != null)
                {
                    redis.Expire(key, (int)expiration.Value.TotalSeconds);
                }
            }
        }

        private static List<string> ExecuteQueryAndGetResultList(string query)
        {
            List<string> valuesToAdd = new List<string>();
            using (SqlConnection connection = new SqlConnection("context connection=true"))
            {
                connection.Open();
                valuesToAdd.Add(GetColumnMetadataXml(connection, query));
                using (SqlCommand queryCommand = new SqlCommand(query, connection))
                using (SqlDataReader rowsetReader = queryCommand.ExecuteReader())
                {
                    while (rowsetReader.Read())
                    {
                        XElement el = new XElement("item");
                        for (int index = 0; index < rowsetReader.FieldCount; index++)
                        {
                            if (!rowsetReader.IsDBNull(index))
                                el.Add(new XElement(rowsetReader.GetName(index), rowsetReader.GetValue(index)));
                        }
                        valuesToAdd.Add(el.ToString());
                    }
                }
            }
            return valuesToAdd;
        }

        private static string GetColumnMetadataXml(SqlConnection connection, string query)
        {
            using (var cmd = new SqlCommand("sp_describe_first_result_set", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("tsql", query);
                XElement el = new XElement("ColumnsMetadata");
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        el.Add(new XElement("Column",
                            new XAttribute("order", reader["column_ordinal"]),
                            new XAttribute("name", reader["name"]),
                            new XAttribute("sqlType", reader["system_type_name"])));
                    }
                }
                return el.ToString();
            }
        }

        public static string RowsetMagic => "REDISQLROWSET";
    }
}
