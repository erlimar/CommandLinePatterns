using System;

namespace CommandLinePattern
{
    public class ProgramOptionAttribute : Attribute
    {
        private readonly ProgramOptionBase _option;
        private readonly bool _defineOption;

        public ProgramOptionAttribute(string name, string pattern, string description)
        {
            _option = new ProgramOptionBase
            {
                Name = name,
                Pattern = pattern,
                Description = description,
                IsFlag = false
            };

            _defineOption = true;
        }

        public ProgramOptionAttribute(string name)
            : this(name, null, null)
        {
            _defineOption = false;
        }

        public ProgramOptionBase Option { get { return _option; } }
        public bool Define { get { return _defineOption; } }
    }
}