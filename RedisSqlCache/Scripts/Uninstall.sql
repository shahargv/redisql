USE [check1]
GO
DROP PROCEDURE [redisql].[SetStringValue]
GO
DROP FUNCTION [redisql].[GetStringValue]
GO
DROP FUNCTION [redisql].[IsKeyExists]
GO
DROP FUNCTION [redisql].[GetSetStringValue]
GO
DROP FUNCTION [redisql].[DeleteKey]
GO
DROP PROCEDURE [redisql].[ConvertXmlToRowset]
GO
DROP FUNCTION [redisql].[SetStringValueIfNotExists]
GO
DROP FUNCTION [redisql].[Rename]
GO
DROP FUNCTION [redisql].[SetRelativeExpiration]
GO
DROP FUNCTION [redisql].[SetExactExpiration]
GO
DROP FUNCTION [redisql].[GetKeyTTL]
GO
DROP PROCEDURE [redisql].[SaveChanges]
GO
DROP PROCEDURE [redisql].[Flush]
GO
DROP FUNCTION [redisql].[GetLastSaved]
GO
DROP FUNCTION [redisql].[GetKeys]
GO
DROP FUNCTION [redisql].[GetInfo]
GO
DROP FUNCTION [redisql].[GetListItems]
GO
DROP FUNCTION [redisql].[GetListItemInIndex]
GO
DROP FUNCTION [redisql].[ListLeftPop]
GO
DROP FUNCTION [redisql].[ListRightPop]
GO
DROP FUNCTION [redisql].[GetListLength]
GO
DROP ASSEMBLY [RediSql]
GO
DROP SCHEMA redisql
GO
