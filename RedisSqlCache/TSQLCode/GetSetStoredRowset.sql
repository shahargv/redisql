
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE redisql.GetSetStoredRowset
	@key nvarchar(250),
	@host nvarchar(50),
	@port int = 6379,
	@password nvarchar(50) = null,
	@dbId int = null,
	@expiration time = null,
	@query nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;

    IF (redisql.GetKeyType(@host, @port, @password, @dbId, @key) = 'Rowset')
	BEGIN
		IF (@expiration IS NOT NULL)
		BEGIN
			SELECT redisql.SetRelativeExpiration(@host, @port, @password, @dbId, @key, @expiration)
		END
		DECLARE @numberOfRows int
		EXECUTE @numberOfRows = redisql.GetStoredRowset @host, @port, @password, @dbId, @key
		RETURN @numberOfRows
	END
	ELSE
	BEGIN
		EXECUTE redisql.StoreQueryResultsData @host, @port, @password, @dbId, @key, @query, @expiration
		EXEC sp_executesql @query
		RETURN -1
	END
END
GO
