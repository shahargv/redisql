using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstallerScriptGenerator.ExternalInterfaces.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class SqlInstallerScriptGeneratorExportedProcedure : SqlInstallerScriptGeneratorExportedAttributeBase
    {
        public SqlInstallerScriptGeneratorExportedProcedure(string functionName, string schemaName)
        {
            Name = functionName;
            SchemaName = schemaName;
        }
    }
}
