using System;
using System.Collections.Generic;
using System.Text;

namespace RedisSqlCache.Common
{
    public static class DateTimeUtils
    {
        private static readonly DateTime BaseLinuxTime = new DateTime(1970, 1, 1);

        public static double ToUnixTime(DateTime dt)
        {
            return (BaseLinuxTime - dt).TotalSeconds;
        }
    }
}
