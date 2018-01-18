using System;

namespace CommandLinePattern
{
    public class InvalidOptionValueException : Exception
    {
        private const string MESSAGE_TEMPLATE = "Invalid value \"{0}\" for option \"{1}\".";

        public InvalidOptionValueException(string optionName, string optionValue)
            : base(string.Format(MESSAGE_TEMPLATE, optionValue, optionName))
        { }
    }
}
