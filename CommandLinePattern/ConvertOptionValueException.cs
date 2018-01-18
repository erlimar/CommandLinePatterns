using System;

namespace CommandLinePattern
{
    public class ConvertOptionValueException : Exception
    {
        private const string MESSAGE_TEMPLATE = "The value of option \"{0}\" can not be converted to \"{1}\". Value: [{2}]";

        public ConvertOptionValueException(string optionName, string typeName, string optionValue)
            : base(string.Format(MESSAGE_TEMPLATE, optionName, typeName, optionValue))
        { }
    }
}
