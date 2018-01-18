using System;

namespace CommandLinePattern
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ProgramAcceptedValuesAttribute : Attribute
    {
        public ProgramAcceptedValuesAttribute(params string[] values)
        {
            Values = values;
        }

        public string[] Values { get; private set; }
    }
}