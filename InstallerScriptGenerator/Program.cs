using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using InstallerScriptGenerator.BO;
using Microsoft.SqlServer.Server;
using SqlClrDeclarations.Attributes;
using SqlClrDeclarations.Attributes;

namespace InstallerScriptGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            string assemblyPath = "C:\\Projects\\RediSQLCache\\RedisSqlCache\\bin\\Debug\\RediSql.dll";
            var asm = Assembly.LoadFile(assemblyPath);
            if (!asm.IsDefined(typeof(SqlInstallerScriptGeneratorExportedAssembly), false))
                throw new Exception();
            var sqlAssembly = new InstallerScriptableSqlAssembly(asm);
            Console.WriteLine(sqlAssembly.GenerateInstallScript());
            StringBuilder installScriptText = new StringBuilder();
            foreach (var method in asm.GetTypes().SelectMany(k => k.GetMethods()).Where(k => k.GetCustomAttributes(false).Any(l => l is SqlInstallerScriptGeneratorExportedAttributeBase)))
            {
                var attribute = method.GetCustomAttribute<SqlInstallerScriptGeneratorExportedAttributeBase>();
                var scriptableItem = InstallerScriptableItem.GetScreiptableItem(attribute, sqlAssembly, method);
                string str = scriptableItem.GenerateInstallScript();
                installScriptText.AppendLine(str);
               
            }
        }
    }
}
