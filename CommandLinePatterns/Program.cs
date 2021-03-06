﻿using CommandLinePattern;
using System;
using System.Diagnostics;

namespace CommandLinePatterns
{
    using static Constants;
    using static UnknownOptionAction;

    public class Program : ProgramDescription
    {
        public Program(IProgramConsole console) : base(console, PROGRAM_NAME, PROGRAM_DESCRIPTION, PROGRAM_SYNOPSIS)
        {
            UnknownOptionAction = ByPass;

            Spec
               .Option("Command", "-c|--command", "The command name")
               .Option("UserName", "--username", "The user name")
               .Flag("TestOnly", "-t|--test-only", "Run on test mode");
        }

        //[ProgramOption("Command", "-c|--command", "The command name")]
        [ProgramOption("Command")]
        [ProgramOptionValues(
            "help   | The help command",
            "env    | The environment command",
            "update | The update command"
        )]
        public string Command { get; set; }

        [ProgramOption("UpdateChannel", "--channel", "The update channel name")]
        [ProgramOptionValue("alpha")]
        [ProgramOptionValue("Beta")]
        [ProgramOptionValue("RELEASE")]
        public string UpdateChannel { get; set; }

        //[ProgramFlag("TestOnly", "-t|--test-only", "Run on test mode")]
        [ProgramFlag("TestOnly")]
        public bool TestOnly { get; set; }

        //[ProgramOption("UserName", "--username", "The user name")]
        [ProgramOption("UserName")]
        public string UserName { get; set; }

        [ProgramOption("UserPassword", "-px|--password", "The user password")]
        public string UserPassword { get; set; }

        /// <summary>
        /// The program entry point.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        public static void Main(string[] args)
        {
            Debug.Print(string.Join(Environment.NewLine, args));
            
            IProgramConsole console = new ProgramConsole();
            var program = Parse(new Program(console), args);

            if (program.EnsureShowHelp())
            {
                return;
            }

            Console.WriteLine("Hello world!");
        }
    }
}
