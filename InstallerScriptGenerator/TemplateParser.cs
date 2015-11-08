using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using InstallerScriptGenerator.BO;
using SqlClrDeclarations.Attributes;

namespace InstallerScriptGenerator
{
    public class TemplateParser
    {
        private FileInfo _templateFile;

        public TemplateParser(string filePath)
        {
            _templateFile = new FileInfo(filePath);
        }

        public string ParseTemplate()
        {
            StringBuilder sb = new StringBuilder((int)_templateFile.Length);
            using (var reader = _templateFile.OpenText())
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line == null) break;
                    var installScriptMagic = "~~~InstallScript:~~~";
                    var uninstallScriptMagic = "~~~UninstallScript:~~~";
                    var includeFileMagic = "~~~IncludeFile:~~~";
                    if (line.StartsWith(installScriptMagic))
                    {
                        string assemblyPath = line.Remove(0, installScriptMagic.Length);
                        string installScript = GetInstallScriptText(assemblyPath);
                        sb.AppendLine(installScript);
                    }
                    else if (line.StartsWith(uninstallScriptMagic))
                    {
                        string assemblyPath = line.Remove(0, uninstallScriptMagic.Length);
                        string uninstallScript = GetUninstallScriptText(assemblyPath);
                        sb.AppendLine(uninstallScript);
                    }
                    else if (line.StartsWith(includeFileMagic))
                    {
                        string filePath = line.Remove(0, includeFileMagic.Length);
                        string fileToInclude = File.ReadAllText(filePath);
                        sb.AppendLine(fileToInclude);
                    }
                    else
                    {
                        sb.AppendLine(line);
                    }
                }
            }
            return sb.ToString();
        }

        private string GetUninstallScriptText(string assemeblyPath)
        {
            StringBuilder sb = new StringBuilder();
            var asm = Assembly.LoadFile(assemeblyPath);

            IterateScriptableItemsInsideAssembly(asm, item => sb.AppendLine(item.GenerateUninstallScript()));
            sb.AppendLine(new InstallerScriptableSqlAssembly(asm).GenerateUninstallScript());
            return sb.ToString();
        }

        private string GetInstallScriptText(string assemeblyPath)
        {
            StringBuilder sb = new StringBuilder();
            var asm = Assembly.LoadFile(assemeblyPath);
            sb.AppendLine(new InstallerScriptableSqlAssembly(asm).GenerateInstallScript());
            IterateScriptableItemsInsideAssembly(asm, item => sb.AppendLine(item.GenerateInstallScript()));
            return sb.ToString();
        }

        private void IterateScriptableItemsInsideAssembly(Assembly asm, Action<InstallerScriptableItem> action)
        {
            var sqlAssembly = new InstallerScriptableSqlAssembly(asm);

            foreach (var method in asm.GetTypes().SelectMany(k => k.GetMembers()).Where(k => k.GetCustomAttributes(false)
                                                                                    .Any(l => l is SqlInstallerScriptGeneratorExportedAttributeBase))
                                                                                    .OrderBy(k => k.Name))
            {
                var attribute = method.GetCustomAttribute<SqlInstallerScriptGeneratorExportedAttributeBase>();
                var scriptableItem = InstallerScriptableItem.GetScreiptableItem(attribute, sqlAssembly, method);
                action(scriptableItem);

            }

        }
    }
}
