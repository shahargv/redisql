using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SqlClrDeclarations.Attributes;

namespace InstallerScriptGenerator.BO
{
    internal class InstallerScriptableSqlAssembly : InstallerScriptableItem
    {
        internal Assembly Assembly { get; set; }
        internal string Path { get; set; }

        public InstallerScriptableSqlAssembly(Assembly assembly)
        {
            var assemblyAttribute = assembly.GetCustomAttribute<SqlInstallerScriptGeneratorExportedAssembly>();
            Name = assemblyAttribute.SqlAssemblyName;
            Path = assembly.Location;
            Assembly = assembly;
        }

        internal override string GenerateInstallScript()
        {
            string template = @"
CREATE ASSEMBLY [$$$name$$$]
AUTHORIZATION [dbo]
FROM '$$$path$$$'
WITH PERMISSION_SET = UNSAFE
";
            return template.Replace("$$$path$$$", Path).Replace("$$$name$$$", Name);
        }

        internal override string GenerateUninstallScript()
        {
            return $"DROP ASSEMBLY [{Name}]";
        }
    }
}
