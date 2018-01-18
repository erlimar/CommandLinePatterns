using System;

namespace CommandLinePattern
{
    public class RepeatedOptionException : Exception
    {
        private const string MESSAGE_TEMPLATE = "Repeated option: {0}{1}";

        public RepeatedOptionException(string optionName, bool isFull)
            : base(string.Format(MESSAGE_TEMPLATE, isFull ? "--" : "-", optionName))
        { }
    }
}
