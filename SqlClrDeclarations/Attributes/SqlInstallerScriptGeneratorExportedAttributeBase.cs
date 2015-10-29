using System;

namespace SqlClrDeclarations.Attributes
{
    public class SqlInstallerScriptGeneratorExportedAttributeBase : Attribute
    {
        public string SchemaName { get; set; }
        public string Name { get; set; }
 
    }
}
