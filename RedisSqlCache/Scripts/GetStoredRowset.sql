
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE redisql.GetStoredRowset
	@host nvarchar(250), @port int = 6379, @password nvarchar(100) = null, @dbId int = null, @key nvarchar(256)
AS
BEGIN
	SET NOCOUNT ON;

	IF OBJECT_ID('tempdb..#dataToPivot') IS NOT NULL DROP TABLE #dataToPivot



	CREATE TABLE #dataToPivot (BatchID uniqueidentifier, KeyName nvarchar(max), Value nvarchar(max))
	INSERT INTO #dataToPivot(BatchID, KeyName, Value)
	SELECT	rowXml.id,
			keyValueSet.Name,
			keyValueSet.Value
	FROM redisql.GetListItems(@host, @port, @password, @dbId, @key, default, default)
	OUTER APPLY	(
					SELECT CAST(Value as xml) xmlData, NEWID() id
				) rowXml
	OUTER APPLY	(
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
