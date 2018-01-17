using System;

namespace CommandLinePattern
{
    public class UnknownOptionException : Exception
    {
        public UnknownOptionException(string optionName, bool isFull)
            : base(string.Format("Unknown option: {0}{1}", isFull ? "--" : "-", optionName))
        { }
    }
}
