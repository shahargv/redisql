using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using InstallerScriptGenerator.BO;
using Microsoft.SqlServer.Server;
using SqlClrDeclarations.Attributes;
using SqlClrDeclarations.Attributes;

namespace InstallerScriptGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            string templateFile = args[0];
            string outputFilePath = args[1];
            TemplateParser parser = new TemplateParser(templateFile);
            string result = parser.ParseTemplate();
            File.WriteAllText(outputFilePath, result);
        }
    }
}
