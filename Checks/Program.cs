using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RediSql.SqlClrComponents;

namespace Checks
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = RedisqlLists.GetListItems("localhost", 6379, null, null, "Sdgsdg", 0 , -1);
        }
    }
}
