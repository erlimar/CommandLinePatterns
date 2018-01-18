using System;

namespace CommandLinePattern
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ProgramOptionValuesAttribute : Attribute
    {
        public ProgramOptionValuesAttribute(params string[] values)
        {
            Values = values;
        }

        public string[] Values { get; private set; }
    }
}