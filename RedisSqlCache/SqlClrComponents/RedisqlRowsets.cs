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

namespace RediSql.SqlClrComponents
{
    public static class RedisqlRowsets
    {
        [SqlFunction(DataAccess = DataAccessKind.Read, IsDeterministic = false)]
        public static void StoreQueryResultsData(string host, int port, string password, int? dbId, string key,
            string query, TimeSpan? expiration, bool replaceExisting)
        {
            List<string> valuesToAdd = new List<string>();
            using (SqlConnection connection = new SqlConnection("context connection=true"))
            {
                connection.Open();
                using (SqlCommand queryCommand = new SqlCommand(query, connection))
                using (SqlDataReader rowsetReader = queryCommand.ExecuteReader())
                {
                    while (rowsetReader.Read())
                    {
                        if (valuesToAdd.Count == 0)
                        {
                            valuesToAdd.Add(GetColumnMetadataXml(rowsetReader));
                        }
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
                valuesToAdd.ForEach(val => RedisqlLists.AddToList(host, port, password, dbId, key, val, true, null));
                if (expiration != null)
                {
                    redis.Expire(key, (int)expiration.Value.TotalSeconds);
                }
            }
        }

        private static string GetColumnMetadataXml(SqlDataReader rowsetReader)
        {

            var schemaTable = rowsetReader.GetSchemaTable();


            XElement el = new XElement("ColumnsMetadata");
            foreach (DataRow row in schemaTable.Rows)
            {
                el.Add(new XElement("Column",
                                            new XAttribute("order", row["ColumnOrdinal"]),
                                            new XAttribute("name", row["ColumnName"]),
                                            new XAttribute("sqlType", row["DataTypeName"]),
                                            new XAttribute("size", row["ColumnSize"])));
            }
            return el.ToString();
        }
    }
}
