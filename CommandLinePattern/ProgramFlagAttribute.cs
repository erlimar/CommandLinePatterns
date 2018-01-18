namespace CommandLinePattern
{
    public class ProgramFlagAttribute : OptionAttribute
    {
        public ProgramFlagAttribute(string name, string pattern, string description)
            : base(name, pattern, description)
        { }

        public ProgramFlagAttribute(string name)
            : base(name)
        { }
    }
}