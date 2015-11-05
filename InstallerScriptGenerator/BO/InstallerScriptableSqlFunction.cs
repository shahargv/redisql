using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;
using SqlClrDeclarations.Attributes;

namespace InstallerScriptGenerator.BO
{
    internal class InstallerScriptableSqlFunction : InstallerScriptableClrMethod
    {
        private readonly SqlInstallerScriptGeneratorExportedFunction _exportedFunctionAttribute;
        private readonly SqlFunctionAttribute _sqlFunctionAttribute;
        public InstallerScriptableSqlFunction(string name, string schemaName, InstallerScriptableSqlAssembly sqlSqlAssembly, MethodInfo method) : base(name, schemaName, sqlSqlAssembly, method.DeclaringType, method)
        {
            _exportedFunctionAttribute = method.GetCustomAttribute<SqlInstallerScriptGeneratorExportedFunction>();
            _sqlFunctionAttribute = method.GetCustomAttribute<SqlFunctionAttribute>();
        }

        internal override string GenerateInstallScript()
        {
            StringBuilder sb = new StringBuilder(@"
CREATE FUNCTION $$$SchemaName$$$.$$$SqlFunctionName$$$($$$ParametersList$$$)
RETURNS $$$SqlReturnValueType$$$
AS EXTERNAL NAME[$$$SqlAssemblyName$$$].[$$$FullClrTypeName$$$].[$$$MethodName$$$]
");
            sb.Replace("$$$SchemaName$$$", SchemaName);
            sb.Replace("$$$SqlFunctionName$$$", Name);
            sb.Replace("$$$ParametersList$$$", SqlParameter.GenerateSqlParameterString(Method.GetParameters()));
            sb.Replace("$$$SqlReturnValueType$$$", GetSqlReturnValueType());
            sb.Replace("$$$SqlAssemblyName$$$", SqlAssembly.Name);
            sb.Replace("$$$FullClrTypeName$$$", ContainedType.FullName);
            sb.Replace("$$$MethodName$$$", Method.Name);
            return sb.ToString();
        }

        private string GetSqlReturnValueType()
        {
            if (!string.IsNullOrEmpty(_sqlFunctionAttribute.TableDefinition))
                return $"table({_sqlFunctionAttribute.TableDefinition})";
            if (!string.IsNullOrEmpty(_exportedFunctionAttribute.SqlReturnType))
                return _exportedFunctionAttribute.SqlReturnType;
            return Utils.ClrSqlTermsConverter.ConvertClrTypeToSqlTypeName(Method.ReturnType);
        }



        internal override string GenerateUninstallScript()
        {
            return $"DROP FUNCTION {SchemaName}.{Name}";
        }
    }
}
