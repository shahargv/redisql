using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstallerScriptGenerator.BO;

namespace InstallerScriptGenerator
{
    internal class InstallerScriptBuilder
    {
        private string _installerTemplate;
        private StringBuilder installerScriptString = new StringBuilder();
        private StringBuilder functionsAndProceduresString = new StringBuilder();

        internal InstallerScriptBuilder(string installerTemplate, SqlClrExportedMember exportedMemberSettings)
        {
            _installerTemplate = installerTemplate;
        }

        internal void Add(ExportedMemberFunction memberFunction)
        {
            string functionDeclartionTemplate = @"

CREATE FUNCTION $schemaName$.$functionName($parameters)
RETURNS bit
AS EXTERNAL NAME [$assemblyName$].[$declarationPath$].[$methodName$]
GO

";
        }
        
    }
}
