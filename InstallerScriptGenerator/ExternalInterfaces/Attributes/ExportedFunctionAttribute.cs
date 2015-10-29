using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstallerScriptGenerator.ExternalInterfaces.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SqlInstallerScriptGeneratorExportedFunction : SqlInstallerScriptGeneratorExportedAttributeBase
    {
        public string SqlReturnType { get; set; }

        public SqlInstallerScriptGeneratorExportedFunction(string functionName, string schemaName)
        {
            Name = functionName;
            SchemaName = schemaName;
        }
    }
}
