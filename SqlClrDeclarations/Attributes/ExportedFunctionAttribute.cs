using System;

namespace SqlClrDeclarations.Attributes
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
