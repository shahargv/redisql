using System;
using System.Reflection;
using RediSql.Sql.Attributes;

namespace InstallerScriptGenerator.BO
{
    class ExportedMemberFunction : SqlClrExportedMember
    {

        internal MethodInfo Method { get; private set; }

        public ExportedMemberFunction(ClrSqlExportedClassAttribute exportedClassInfo, Type declaringType, MethodInfo method) : base(exportedClassInfo, declaringType)
        {
            Method = method;
        }
    }
}