using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InstallerScriptGenerator.BO
{
    internal abstract class InstallerScriptableClrMethod : InstallerScriptableItem
    {
        protected Type ContainedType { get; set; }
        protected MethodInfo Method { get; set; }
        internal InstallerScriptableClrMethod(string name, string schemaName, InstallerScriptableSqlAssembly sqlAssembly, Type containedType, MethodInfo method)
        {
            Name = name;
            SchemaName = schemaName;
            ContainedType = containedType;
            Method = method;
            SqlAssembly = sqlAssembly;
        }

        
    }
}
