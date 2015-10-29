using System;

namespace SqlClrDeclarations.Attributes
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
