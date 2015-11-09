using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SqlClrDeclarations.Attributes;

namespace InstallerScriptGenerator.BO
{
    internal abstract class InstallerScriptableItem
    {
        internal string SchemaName { get; set; }
        internal string Name { get; set; }
        public InstallerScriptableSqlAssembly SqlAssembly { get; set; }

        internal abstract string GenerateInstallScript();
        internal abstract string GenerateUninstallScript();

        internal static InstallerScriptableItem GetScreiptableItem(
            SqlInstallerScriptGeneratorExportedAttributeBase attributeInfo, InstallerScriptableSqlAssembly sqlAssembly,
            MemberInfo declaredItem)
        {
            InstallerScriptableItem item = null;
            if (attributeInfo is SqlInstallerScriptGeneratorExportedFunction && declaredItem is MethodInfo)
            {
                item = new InstallerScriptableSqlFunction(attributeInfo.Name, attributeInfo.SchemaName, sqlAssembly,
                    (MethodInfo)declaredItem);
            }
            if (attributeInfo is SqlInstallerScriptGeneratorExportedProcedure && declaredItem is MethodInfo)
            {
                item = new InstallerScriptableSqlProcedure(attributeInfo.Name, attributeInfo.SchemaName, sqlAssembly,
                    (MethodInfo)declaredItem);
            }
            if (item == null)
                throw new ArgumentOutOfRangeException(nameof(attributeInfo), "unexpected type of attribute");
            return item;
        }
    }
}