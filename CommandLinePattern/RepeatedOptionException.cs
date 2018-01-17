using System;

namespace CommandLinePattern
{
    public class RepeatedOptionException : Exception
    {
        public RepeatedOptionException(string optionName, bool isFull)
            : base(string.Format("Repeated option: {0}{1}", isFull ? "--" : "-", optionName))
        { }
    }
}
