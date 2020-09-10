using System;

namespace CommonEntity.Common
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public string Name { get; set; }

        public bool Ignore { get; set; }

        public TableAttribute(string name)
        {
            Name = name;
        }
    }
}