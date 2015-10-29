using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstallerScriptGenerator.ExternalInterfaces.Attributes
{
    public class SqlInstallerScriptGeneratorExportedAttributeBase : Attribute
    {
        public string SchemaName { get; set; }
        public string Name { get; set; }
 
    }
}
