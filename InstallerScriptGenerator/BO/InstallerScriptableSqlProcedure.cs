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
    internal class InstallerScriptableSqlProcedure : InstallerScriptableClrMethod
    {
        private readonly SqlInstallerScriptGeneratorExportedProcedure _exportedProcedureAttribute;
        private readonly SqlProcedureAttribute _sqlProcedureAttribute;
        public InstallerScriptableSqlProcedure(string name, string schemaName, InstallerScriptableSqlAssembly sqlSqlAssembly, MethodInfo method) : base(name, schemaName, sqlSqlAssembly, method.DeclaringType, method)
        {
            _exportedProcedureAttribute = method.GetCustomAttribute<SqlInstallerScriptGeneratorExportedProcedure>();
            _sqlProcedureAttribute = method.GetCustomAttribute<SqlProcedureAttribute>();
        }

        internal override string GenerateInstallScript()
        {
            StringBuilder sb = new StringBuilder(@"
CREATE PROCEDURE $$$SchemaName$$$.$$$SqlProcedureName$$$($$$ParametersList$$$)
AS EXTERNAL NAME[$$$SqlAssemblyName$$$].[$$$FullClrTypeName$$$].[$$$MethodName$$$]
");
            sb.Replace("$$$SchemaName$$$", SchemaName);
            sb.Replace("$$$SqlProcedureName$$$", Name);
            sb.Replace("$$$ParametersList$$$", SqlParameter.GenerateSqlParameterString(Method.GetParameters()));
            sb.Replace("$$$SqlAssemblyName$$$", SqlAssembly.Name);
            sb.Replace("$$$FullClrTypeName$$$", ContainedType.FullName);
            sb.Replace("$$$MethodName$$$", Method.Name);
            return sb.ToString();
        }

    
        internal override string GenerateUninstallScript()
        {
            return $"DROP PROCEDURE {SchemaName}.{Name}";
        }
    }
}
