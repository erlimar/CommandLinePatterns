using System;

namespace CommandLinePattern
{
    public class OptionAttribute : Attribute
    {
        private readonly string _name;
        private readonly string _pattern;
        private readonly string _description;
        private readonly bool _define;

        public OptionAttribute(string name, string pattern, string description)
        {
            _name = name;
            _pattern = pattern;
            _description = description;
            _define = true;
        }

        public OptionAttribute(string name)
            : this(name, null, null)
        {
            _define = false;
        }

        public string Name { get { return _name; } }
        public string Pattern { get { return _pattern; } }
        public string Description { get { return _description; } }
        public bool Define { get { return _define; } }
    }
}