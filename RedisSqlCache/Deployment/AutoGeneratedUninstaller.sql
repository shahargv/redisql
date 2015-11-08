DROP PROCEDURE redisql.AddToList
DROP FUNCTION redisql.DeleteKey
DROP PROCEDURE redisql.Flush
DROP FUNCTION redisql.GetServerInfo
DROP FUNCTION redisql.GetKeys
DROP FUNCTION redisql.GetKeyTTL
DROP FUNCTION redisql.GetKeyType
DROP FUNCTION redisql.GetLastSaved
DROP FUNCTION redisql.GetListItemsAtIndex
DROP FUNCTION redisql.GetListItems
DROP FUNCTION redisql.GetListLength
DROP FUNCTION redisql.GetSetStringValue
DROP FUNCTION redisql.GetStringValue
DROP FUNCTION redisql.IsKeyExists
DROP FUNCTION redisql.ListLeftPop
DROP FUNCTION redisql.RenameKey
DROP FUNCTION redisql.ListRightPop
DROP PROCEDURE redisql.SaveChanges
DROP FUNCTION redisql.SetExactExpiration
DROP FUNCTION redisql.SetRelativeExpiration
DROP PROCEDURE redisql.SetStringValue
DROP FUNCTION redisql.SetStringValueIfNotExists
DROP PROCEDURE redisql.StoreQueryResultsData
DROP ASSEMBLY [RediSql]

GO
DROP ASSEMBLY [SqlClrDeclarations]

GO
DROP PROCEDURE [redisql].[GetStoredRowset]
GO
DROP PROCEDURE [redisql].[ConvertXmlToRowset]
GO
DROP SCHEMA [redisql]
GO
