using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstallerScriptGenerator.Utils
{
    internal static class ClrSqlTermsConverter
    {
        internal static string ConvertClrTypeToSqlTypeName(Type t)
        {
            if (t == typeof(int)) return "int";
            if (t == typeof(long)) return "bigint";
            if (t == typeof(short)) return "smallint";
            if (t == typeof(DateTime)) return "datetime2";
            if (t == typeof(TimeSpan)) return "time";
            if (t == typeof(string)) return "nvarchar(max)";
            throw new ArgumentOutOfRangeException("unknown type: " + t.FullName);
        }
    }
}
