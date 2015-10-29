using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstallerScriptGenerator.ExternalInterfaces.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class SqlParameterAttribute : Attribute
    {
        public string DefaultValue { get; set; }
        public string Name { get; set; }
        public string SqlType { get; set; }
    }
}
