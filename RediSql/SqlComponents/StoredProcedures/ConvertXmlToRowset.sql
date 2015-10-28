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