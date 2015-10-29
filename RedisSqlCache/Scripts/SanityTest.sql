--TEST 1 : non existent key test
DECLARE @nonExistentKeyExists bit =  [redisql].[IsKeyExists] (
   'localhost'
  ,6379
  ,default
  ,default
  ,CAST(NEWID() as varchar(50))
)
IF @nonExistentKeyExists = 1
BEGIN
	PRINT 'ERROR: non existent key test'
END
ELSE
BEGIN
	PRINT 'success: non existent key test'
END
GO

--TEST 2: add and remove key
DECLARE @test2KeyName varchar(50) = CAST(NEWID() as varchar(50))
EXEC [redisql].[SetStringValue]
		@host = N'localhost',
		@port = 6379,
		@key = @test2KeyName,
		@value = N'gv'
IF (SELECT [redisql].[IsKeyExists] ('localhost',default,default,default,@test2KeyName)) = 0
BEGIN
	PRINT 'ERROR (test 2):add key and check if exists'
END
ELSE
BEGIN
	PRINT 'success: test 2 - key added successfully'
END

EXEC [redisql].DeleteKey
	@host='localhost',
	@key=@test2KeyName
IF (SELECT [redisql].[IsKeyExists] ('localhost',default,default,default,@test2KeyName)) = 0
BEGIN
	PRINT 'success (test 2): key removed successfully'
END
ELSE
BEGIN
	PRINT 'ERROR: test 2 - key exists after removing'
END
GO
--TEST 3: add key with expiration
DECLARE @test3KeyName varchar(50) = CAST(NEWID() as varchar(50))
EXEC [redisql].[SetStringValue]
		@host = N'localhost',
		@port = 6379,
		@key = @test3KeyName,
		@value = N'gv',
		@expiration = '00:00:10'
IF (SELECT [redisql].[IsKeyExists] ('localhost',default,default,default,@test3KeyName)) = 0
BEGIN
	PRINT 'ERROR:add key with expiration (test 3) - key not exists after adding it'
END
WAITFOR DELAY '00:00:11'
IF (SELECT [redisql].[IsKeyExists] ('localhost',default,default,default,@test3KeyName)) = 1
BEGIN
	PRINT 'ERROR:add key with expiration (test 3) - key exists after expiration time'
END
ELSE
BEGIN
	PRINT 'success: add key and check if exists with expiration (test 3)'
END
GO
--TEST 4: Check lists
DECLARE @test4KeyName varchar(50) = CAST(NEWID() as varchar(50))
EXEC [redisql].AddToList
		@host = N'localhost',
		@port = 6379,
		@key = @test4KeyName,
		@value = N'val1'
EXEC [redisql].AddToList
		@host = N'localhost',
		@port = 6379,
		@key = @test4KeyName,
		@value = N'val2'
EXEC [redisql].AddToList
		@host = N'localhost',
		@port = 6379,
		@key = @test4KeyName,
		@value = N'val3'
DECLARE @numberOfItemsInResults int = (SELECT COUNT(*) FROM redisql.GetListItems('localhost', default, default, default, @test4KeyName, default, default))
IF @numberOfItemsInResults <> 3
BEGIN
	PRINT 'ERROR: lists test (test 4)'
END
ELSE
BEGIN
	PRINT 'success: lists test (test 4)'
END
--TEST 5: Rowset storing
DECLARE @test5KeyName varchar(50) =  CAST(NEWID() as varchar(50))

IF OBJECT_ID('tempdb..#test5') IS NOT NULL DROP TABLE #test5
CREATE TABLE #test5(col1 nvarchar(200), col2 varchar(200), col3 nvarchar(max) null, col4 int null)
INSERT INTO #test5(col1, col2, col3, col4) VALUES ('c1val', 'c2val', 'c3val', 9)
INSERT INTO #test5(col1, col2, col3, col4) VALUES ('c1val', 'c2val', null, 90)
INSERT INTO #test5(col1, col2, col3, col4) VALUES ('c1val', 'c2val', 'c3val', 91)
INSERT INTO #test5(col1, col2, col3, col4) VALUES ('c1val', 'c2val', 'c3val', null)
INSERT INTO #test5(col1, col2, col3, col4) VALUES ('c1val', 'c2val', 'c3val', null)
EXEC [redisql].StoreQueryResultsData
		@host = N'localhost',
		@port = 6379,
		@key = @test5KeyName,
		@query = N'SELECT * FROM #test5',
		@replaceExisting = 1
DECLARE @backFromCache table (col1 nvarchar(200), col2 varchar(200), col3 nvarchar(max) null, col4 int null) 
IF OBJECT_ID('tempdb..#test5_b') IS NOT NULL DROP TABLE #test5_b
CREATE TABLE #test5_b(col1 nvarchar(200), col2 varchar(200), col3 nvarchar(max) null, col4 int null)
INSERT INTO #test5_b(col1,col2,col3,col4)  EXEC [redisql].[GetStoredRowset]		@host = N'localhost',		@key =@test5KeyName
IF(SELECT COUNT(*) FROM #test5_b) <> 5
BEGIN
	PRINT 'ERROR: rowset test - incorrect number of total returned rows (test5)'
END
ELSE
BEGIN
	PRINT 'success: rowset test - correct number of total returned rows (test5)'
END
IF(SELECT COUNT(*) FROM #test5_b WHERE col4 IS NULL) <> 2
BEGIN
	PRINT 'ERROR: rowset test - incorrect number of null returned rows (test5)'
END
ELSE
BEGIN
	PRINT 'success: rowset test - correct number of null returned rows (test5)'
END