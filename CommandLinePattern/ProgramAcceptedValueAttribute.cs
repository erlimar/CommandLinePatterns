using System;

namespace CommandLinePattern
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ProgramAcceptedValueAttribute : ProgramAcceptedValuesAttribute
    {
        public ProgramAcceptedValueAttribute(string value)
            : base(value)
        { }
    }
}