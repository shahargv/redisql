DECLARE @dbName nvarchar(50) = '[check1]'

USE check1

EXEC('ALTER DATABASE ' + @dbname + ' SET TRUSTWORTHY ON')
GO
CREATE ASSEMBLY [RedisSqlCache]
AUTHORIZATION [dbo]
FROM 'C:\Projects\RediSQLCache\RedisSqlCache\bin\Debug\RedisSqlCache.dll'
WITH PERMISSION_SET = UNSAFE
GO

GO
CREATE SCHEMA [redisql] AUTHORIZATION [dbo]
GO

CREATE FUNCTION redisql.IsKeyExists(@host nvarchar(250), @port int = 6379, @password nvarchar(100) = null, @dbId int = null, @key nvarchar(250))
RETURNS bit
AS EXTERNAL NAME [RedisSqlCache].[RedisSqlCache.Sql.Functions.RedisSqlFunctions].[IsKeyExists]
GO

CREATE PROCEDURE redisql.SetStringValue(@host nvarchar(250), @port int = 6379, @password nvarchar(100) = null, @dbId int = null, @key nvarchar(250), @value nvarchar(max), @expiration time = null)
AS EXTERNAL NAME [RedisSqlCache].[RedisSqlCache.Sql.Functions.RedisSqlFunctions].[SetStringValue]
GO

CREATE FUNCTION redisql.GetStringValue(@host nvarchar(250), @port int = 6379, @password nvarchar(100) = null, @dbId int = null, @key nvarchar(250))
RETURNS nvarchar(max)
AS EXTERNAL NAME [RedisSqlCache].[RedisSqlCache.Sql.Functions.RedisSqlFunctions].[GetStringValue]

GO

CREATE FUNCTION redisql.GetSetStringValue(@host nvarchar(250), @port int = 6379, @password nvarchar(100) = null, @dbId int = null, @key nvarchar(250), @value nvarchar(max), @expiration time = null)
RETURNS nvarchar(max)
AS EXTERNAL NAME [RedisSqlCache].[RedisSqlCache.Sql.Functions.RedisSqlFunctions].[GetSetStringValue]
GO

CREATE FUNCTION redisql.DeleteKey(@host nvarchar(250), @port int = 6379, @password nvarchar(100) = null, @dbId int = null, @key nvarchar(250))
RETURNS bit
AS EXTERNAL NAME [RedisSqlCache].[RedisSqlCache.Sql.Functions.RedisSqlFunctions].[DeleteKey]
GO

CREATE FUNCTION redisql.SetStringValueIfNotExists(@host nvarchar(250), @port int = 6379, @password nvarchar(100) = null, @dbId int = null, @key nvarchar(250), @value nvarchar(max), @expiration time = null)
RETURNS bit
AS EXTERNAL NAME [RedisSqlCache].[RedisSqlCache.Sql.Functions.RedisSqlFunctions].[SetStringValueIfNotExists]
GO

CREATE FUNCTION redisql.Rename(@host nvarchar(250), @port int = 6379, @password nvarchar(100) = null, @dbId int = null, @key nvarchar(250), @keyNewName nvarchar(250))
RETURNS bit
AS EXTERNAL NAME [RedisSqlCache].[RedisSqlCache.Sql.Functions.RedisSqlFunctions].[Rename]
GO

CREATE FUNCTION redisql.SetRelativeExpiration(@host nvarchar(250), @port int = 6379, @password nvarchar(100) = null, @dbId int = null, @key nvarchar(250), @expiration time)
RETURNS bit
AS EXTERNAL NAME [RedisSqlCache].[RedisSqlCache.Sql.Functions.RedisSqlFunctions].[SetRelativeExpiration]
GO

CREATE FUNCTION redisql.SetExactExpiration(@host nvarchar(250), @port int = 6379, @password nvarchar(100) = null, @dbId int = null, @key nvarchar(250), @expiration datetime)
RETURNS bit
AS EXTERNAL NAME [RedisSqlCache].[RedisSqlCache.Sql.Functions.RedisSqlFunctions].[SetExactExpiration]
GO

CREATE FUNCTION redisql.GetKeyTTL(@host nvarchar(250), @port int = 6379, @password nvarchar(100) = null, @dbId int = null, @key nvarchar(250))
RETURNS int
AS EXTERNAL NAME [RedisSqlCache].[RedisSqlCache.Sql.Functions.RedisSqlFunctions].[GetKeyTTL]
GO

CREATE PROCEDURE redisql.[SaveChanges](@host nvarchar(250), @port int = 6379, @password nvarchar(100) = null, @dbId int = null, @isBackground bit = 0)
AS EXTERNAL NAME [RedisSqlCache].[RedisSqlCache.Sql.Functions.RedisSqlFunctions].[Save]
GO

CREATE PROCEDURE redisql.Flush(@host nvarchar(250), @port int = 6379, @password nvarchar(100) = null, @dbId int = null)
AS EXTERNAL NAME [RedisSqlCache].[RedisSqlCache.Sql.Functions.RedisSqlFunctions].[Flush]
GO

CREATE FUNCTION redisql.GetLastSaved(@host nvarchar(250), @port int = 6379, @password nvarchar(100) = null, @dbId int = null)
RETURNS datetime
AS EXTERNAL NAME [RedisSqlCache].[RedisSqlCache.Sql.Functions.RedisSqlFunctions].[GetLastSaved]
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE redisql.ConvertXmlToRowset
(
	@input xml
)
AS
BEGIN
	IF OBJECT_ID('tempdb..#dataToPivot') IS NOT NULL DROP TABLE #dataToPivot

	CREATE TABLE #dataToPivot (BatchID uniqueidentifier, KeyName nvarchar(max), Value nvarchar(max))
	INSERT INTO #dataToPivot(BatchID, KeyName, Value)

	SELECT	rowXml.ID,
			keyValueSet.Name,
			keyValueSet.Value
	FROM   @input.nodes('/items/item') T(c)
	OUTER APPLY (
					SELECT CAST(T.c.query('.') as xml) xmlData,
					NEWID() id
				)  rowXml
	OUTER APPLY (
					SELECT
						C.Name,
						C.Value
					FROM rowXml.xmlData.nodes('/item/*') as T(C)
					OUTER APPLY (
									SELECT
										T.C.value('local-name(.)', 'nvarchar(max)') as Name,
										T.C.value('(./text())[1]', 'nvarchar(max)') as Value
									UNION ALL
									SELECT
										A.C.value('local-name(.)', 'nvarchar(max)') as Name,
										A.C.value('.', 'nvarchar(max)') as Value
									FROM T.C.nodes('@*') as A(C)
								) as C
					where C.Value is not null 
				) keyValueSet

	DECLARE @colsNames NVARCHAR(2000)
	SELECT  @colsNames = STUFF(( SELECT DISTINCT TOP 100 PERCENT
									'],[' + t2.keyName
							FROM    #dataToPivot AS t2
							ORDER BY '],[' + t2.keyName
							FOR XML PATH('')
						  ), 1, 2, '') + ']'

	DECLARE @query NVARCHAR(4000)
	SET @query = N'SELECT '+
	@colsNames +'
	FROM
	(SELECT  t2.BatchID
		  , t1.KeyName
		  , t1.Value
	FROM    #dataToPivot AS t1
			JOIN #dataToPivot AS t2 ON t1.BatchID = t2.BatchID) p
	PIVOT
	(
	MAX([Value])
	FOR KeyName IN
	( '+
	@colsNames +' )
	) AS pvt
	ORDER BY BatchID;'
	EXECUTE(@query)

END


GO

GO
sp_configure 'show advanced options', 1;
GO
RECONFIGURE;
GO
sp_configure 'clr enabled', 1;
GO
RECONFIGURE;
GO