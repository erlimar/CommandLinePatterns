using System;

namespace CommandLinePattern
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ProgramOptionValueAttribute : ProgramOptionValuesAttribute
    {
        public ProgramOptionValueAttribute(string value)
            : base(value)
        { }
    }
}