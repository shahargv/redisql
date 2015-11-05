
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE redisql.GetStoredRowset
	@host nvarchar(250), @port int = 6379, @password nvarchar(100) = null, @dbId int = null, @key nvarchar(256)
AS

BEGIN
	SET NOCOUNT ON;

	IF OBJECT_ID('tempdb..#items') IS NOT NULL DROP TABLE #items

	CREATE TABLE #items(val nvarchar(max))
	INSERT INTO #items(val)
	SELECT Value 
	FROM redisql.GetListItems(@host, @port, @password, @dbId, @key, default, default)

	DELETE TOP (1)
	FROM   #items

	DECLARE @metadataXml xml = (SELECT TOP 1 cast(val as xml) FROM #items)
	DELETE TOP (1)
	FROM   #items

	DECLARE @columnsMetadata table(Seq int, Name nvarchar(250), DataType varchar(100))
	INSERT INTO @columnsMetadata(Seq, Name, DataType)
	SELECT	col.value('(@order)[1]', 'nvarchar(max)'),
			col.value('(@name)[1]', 'nvarchar(max)'),
			col.value('(@sqlType)[1]', 'nvarchar(max)')
	FROM @metadataXml.nodes('/ColumnsMetadata/Column') as columns(col)

	DECLARE @dynamicSelectors nvarchar(max)
	SELECT @dynamicSelectors =COALESCE(@dynamicSelectors + ', ', '') + 'T.C.value(''/item[1]/' +Name +'[1]'', ''' + DataType + ''') ' + Name 
	FROM @columnsMetadata
	ORDER BY Seq

	DECLARE @sql nvarchar(max) =
	'
		SELECT o.*
		FROM #items
		OUTER APPLY	(
						SELECT CAST(val as xml) xmlData
					) rowXml
		OUTER APPLY	(
						SELECT TOP 1 dataColumns.*
						FROM rowXml.xmlData.nodes(''/item/*'') as T(C)
						OUTER APPLY	(
										SELECT ' + @dynamicSelectors + '
									) dataColumns
					) o
	'
	EXECUTE(@sql)
END
GO