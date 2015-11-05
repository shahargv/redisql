using System;
using System.Collections.Generic;
using System.IO;
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
            var assemblyAttribute = assembly.GetCustomAttribute<AssemblyTitleAttribute>();
            Name = assemblyAttribute.Title;
            Path = assembly.Location;
            Assembly = assembly;
        }

        internal override string GenerateInstallScript()
        {
            string template = @"
CREATE ASSEMBLY [$$$name$$$]
AUTHORIZATION [dbo]
FROM 0x$$$bits$$$
WITH PERMISSION_SET = UNSAFE
";
            return template.Replace("$$$bits$$$", BitConverter.ToString(File.ReadAllBytes(Path)).Replace("-", string.Empty)).Replace("$$$name$$$", Name);
        }

        internal override string GenerateUninstallScript()
        {
            return $"DROP ASSEMBLY [{Name}]";
        }
    }
}
