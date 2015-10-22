using System;
using System.Reflection;
using RediSql.Sql.Attributes;

namespace InstallerScriptGenerator.BO
{
    class ExportedMemberProcedure : SqlClrExportedMember
    {
        
        internal MethodInfo Method { get; private set; }

        public ExportedMemberProcedure(ClrSqlExportedClassAttribute exportedClassInfo, Type declaringType, MethodInfo method) : base(exportedClassInfo, declaringType)
        {
            Method = method;
        }
    }
}