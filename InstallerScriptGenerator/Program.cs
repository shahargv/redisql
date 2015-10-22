using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;
using RediSql.Sql.Attributes;

namespace InstallerScriptGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            string assemblyPath = args[0];
            string sqlInstallerOutputPath = args[1];
            string sqlUninstallerOutputPath = args[2];
            string installerTemplatePath = args[3];
            string uninstallerTemplatePath = args[4];

            string installerTemplateText = File.ReadAllText(installerTemplatePath);

            Assembly asm = Assembly.LoadFile(assemblyPath);
            var sqlClrTypesToInstall =
                asm.GetTypes()
                    .Where(k => k.IsDefined(typeof (ClrSqlExportedClassAttribute)))
                    .Select(k => new 
                    {
                        TypeInfo = k,
                        ExportedClassAttribute = k.GetCustomAttribute<ClrSqlExportedClassAttribute>()
                    });
            foreach (var t in sqlClrTypesToInstall)
            {
                var definedSqlFunctions = t.TypeInfo.GetMethods().Where(k => k.IsDefined(typeof (SqlFunctionAttribute)));
                var defindedSqlProcedures =
                    t.TypeInfo.GetMethods().Where(k => k.IsDefined(typeof (SqlProcedureAttribute)));
                foreach (var sqlFunction in definedSqlFunctions)
                {
                    
                }
            }
        }
    }
}
