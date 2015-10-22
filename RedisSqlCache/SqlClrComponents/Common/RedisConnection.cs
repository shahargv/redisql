namespace RediSql.SqlClrComponents.Common
{
    internal static class RedisConnection
    {
        internal static Redis GetConnection(string host, int port, string password = null, int? dbId = null)
        {
            Redis redis = new Redis(host, port);
            if (password != null)
                redis.Password = password;
            if (dbId != null)
                redis.Db = dbId.Value;
            return redis;
        }
    }
}
