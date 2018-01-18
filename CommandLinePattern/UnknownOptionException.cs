using System;

namespace CommandLinePattern
{
    public class UnknownOptionException : Exception
    {
        private const string MESSAGE_TEMPLATE = "Unknown option: {0}{1}";

        public UnknownOptionException(string optionName, bool isFull)
            : base(string.Format(MESSAGE_TEMPLATE, isFull ? "--" : "-", optionName))
        { }
    }
}
