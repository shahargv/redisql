using System;

namespace SqlClrDeclarations.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class SqlParameterAttribute : Attribute
    {
        public object DefaultValue { get; set; }
        public string Name { get; set; }
        public string SqlType { get; set; }
    }
}
