using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SqlClrDeclarations.Attributes;

namespace InstallerScriptGenerator.BO
{
    internal class SqlParameter
    {
        internal string Name { get; set; }
        internal object DefaultValue { get; set; }

        internal string SqlDefaultValue
        {
            get
            {
                if (DefaultValue == null) return null;
                if (DefaultValue == DBNull.Value) return "null";
                if (DefaultValue == typeof(DBNull)) return "null";
                if (SqlType == Utils.ClrSqlTermsConverter.ConvertClrTypeToSqlTypeName(typeof(string))) return "'" + DefaultValue + "'";
                return DefaultValue.ToString();
            }
        }
        internal string SqlType { get; set; }
        internal SqlParameter(ParameterInfo parameter)
        {
            if (parameter.IsDefined(typeof(SqlParameterAttribute)))
            {
                var attribute = (SqlParameterAttribute)parameter.GetCustomAttribute(typeof(SqlParameterAttribute));
                Name = !string.IsNullOrEmpty(attribute.Name) ? attribute.Name : parameter.Name;
                SqlType = !string.IsNullOrEmpty(attribute.SqlType)
                    ? attribute.SqlType
                    : Utils.ClrSqlTermsConverter.ConvertClrTypeToSqlTypeName(parameter.ParameterType);
                DefaultValue = attribute.DefaultValue;
                if (DefaultValue is bool)
                    DefaultValue = (bool)attribute.DefaultValue ? 0 : 1;

            }
            else
            {
                DefaultValue = null;
                Name = parameter.Name;
                SqlType = Utils.ClrSqlTermsConverter.ConvertClrTypeToSqlTypeName(parameter.ParameterType);
            }
        }

        internal static string GenerateSqlParameterString(ParameterInfo[] parameters)
        {
            return string.Join(", ", parameters.Select(param =>
            {
                var paramInfo = new SqlParameter(param);
                string singleParam = $"@{paramInfo.Name} {paramInfo.SqlType}";
                if (paramInfo.SqlDefaultValue != null)
                    singleParam += $"={paramInfo.SqlDefaultValue}";
                return singleParam;
            }));
        }

    }
}
