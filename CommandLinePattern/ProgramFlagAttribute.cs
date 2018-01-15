using System;

namespace CommandLinePattern
{
    public class ProgramFlagAttribute : Attribute
    {
        private readonly ProgramOptionBase _flag;
        private readonly bool _defineFlag;

        public ProgramFlagAttribute(string name, string pattern, string description)
        {
            _flag = new ProgramOptionBase
            {
                Name = name,
                Pattern = pattern,
                Description = description,
                IsFlag = true
            };

            _defineFlag = true;
        }

        public ProgramFlagAttribute(string name)
            : this(name, null, null)
        {
            _defineFlag = false;
        }

        public ProgramOptionBase Flag { get { return _flag; } }
        public bool Define { get { return _defineFlag; } }
    }
}