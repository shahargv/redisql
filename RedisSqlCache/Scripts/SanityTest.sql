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
