SELECT [redisql].[IsKeyExists] (
   'localhost'
  ,6379
  ,default
  ,default
  ,'check1'
)

GO

EXEC [redisql].[SetStringValue]
		@host = N'localhost',
		@port = 6379,
		@key = N'check1',
		@value = N'gv'


GO

SELECT [redisql].[GetStringValue] (
   'localhost'
  ,6379
  ,default
  ,default
  ,'check1')

GO