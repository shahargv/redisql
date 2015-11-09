-- This file includes some samples of using RediSql.
-- For more samples, and full documentation, please visit http://redisql.ishahar.net

--Get some server info
SELECT * FROM [redisql].[GetServerInfo] (
   'localhost' --Redis hosted locally
  ,default --default port
  ,default --no password
  ,default --dbid: 0
  )
GO

--Create string key without expiration
EXEC [redisql].[SetStringValue]
		@host = N'localhost',
		@port = 6379,
		@key = 'SimpleStringKey',
		@value = N'sample_value'

--We can check if the key exists now
IF (SELECT [redisql].[IsKeyExists] ('localhost',default,default,default,'SimpleStringKey')) = 1
BEGIN
	PRINT 'Key Exists!'
END

--We can add keys with expiration, in the following snippet - 10 minutes
EXEC [redisql].[SetStringValue]
		@host = N'localhost',
		@port = 6379,
		@key = 'StringKeyWithTTL',
		@value = N'gv',
		@expiration = '00:10:00' --10 minutes expiration

--Let's print the value of the key
--if the key not exists we'll get NULL value
SELECT [redisql].[GetStringValue] (
   'localhost'
  ,default --default port
  ,default -- no password
  ,default -- dbid: 0
  ,'SimpleStringKey')


--Get all keys from Redis. 
--not meant for production code, but mostly for debugging
SELECT * FROM [redisql].[GetKeys] (
   'localhost'
  ,default
  ,default
  ,default
  ,'*' -- get all keys, no filter
  )

--Get all keys from Redis that contains "String" in the key name. 
--not meant for production code, but mostly for debugging
SELECT * FROM [redisql].[GetKeys] (
   'localhost'
  ,default
  ,default
  ,default
  ,'*String*' -- get all keys, no filter
  )

--Commit all changes to Redis, by producing point-in-time snapshot
--For further information about persistency in Redis please refer to:
--http://redis.io/topics/persistence
EXEC [redisql].[SaveChanges]
		@host = 'localhost'

--store the result of the query in Redis
EXEC [redisql].StoreQueryResultsData
		@host = N'localhost',
		@port = 6379,
		@key = 'RowsetKey1',
		@query = N'SELECT * FROM INFORMATION_SCHEMA.ROUTINES',
		@replaceExisting = 1

--Get the results of the query we stored before
EXEC [redisql].[GetStoredRowset]		
		@host = N'localhost',		
		@key ='RowsetKey1'