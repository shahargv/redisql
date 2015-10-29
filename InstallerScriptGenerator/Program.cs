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
            var sqlAssembly = new ExportedSqlAssembly(asm);
            Console.WriteLine(sqlAssembly.GenerateInstallScript());
        }
    }
}
