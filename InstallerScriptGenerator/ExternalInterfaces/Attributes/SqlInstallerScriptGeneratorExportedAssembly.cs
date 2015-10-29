using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstallerScriptGenerator.ExternalInterfaces.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class SqlInstallerScriptGeneratorExportedAssembly : Attribute
    {
        public string SqlAssemblyName { get; set; }

        public SqlInstallerScriptGeneratorExportedAssembly(string sqlAssemblyName)
        {
            SqlAssemblyName = sqlAssemblyName;
        }
    }
}
