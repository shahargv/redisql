using System;
using RediSql.Sql.Attributes;

namespace InstallerScriptGenerator.BO
{
    internal abstract class SqlClrExportedMember
    {
        internal SqlClrExportedMember(ClrSqlExportedClassAttribute exportedClassInfo, Type declaringType)
        {
            ExportedClassInfo = exportedClassInfo;
            DeclaringType = declaringType;
        }

        internal Type DeclaringType { get; private set; }
        internal ClrSqlExportedClassAttribute ExportedClassInfo { get; private set; }
    }
}