﻿using System;

namespace CommandLinePattern
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ProgramOptionAttribute : OptionAttribute
    {
        public ProgramOptionAttribute(string name, string pattern, string description)
            : base(name, pattern, description)
        { }

        public ProgramOptionAttribute(string name)
            : base(name)
        { }
    }
}