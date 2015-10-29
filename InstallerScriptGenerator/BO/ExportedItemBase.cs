using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstallerScriptGenerator.BO
{
    internal abstract class ExportedItemBase
    {
        internal string SchemaName { get; set; }
        internal string Name { get; set; }
        public ExportedSqlAssembly SqlAssembly { get; set; }

        internal abstract string GenerateInstallScript();
        internal abstract string GenerateUninstallScript();

    }
}
