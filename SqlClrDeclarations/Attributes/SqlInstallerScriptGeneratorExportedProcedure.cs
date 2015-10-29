using System;

namespace SqlClrDeclarations.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SqlInstallerScriptGeneratorExportedProcedure : SqlInstallerScriptGeneratorExportedAttributeBase
    {
        public SqlInstallerScriptGeneratorExportedProcedure(string functionName, string schemaName)
        {
            Name = functionName;
            SchemaName = schemaName;
        }
    }
}
