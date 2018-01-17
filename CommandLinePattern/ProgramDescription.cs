using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace CommandLinePattern
{
    public class ProgramDescription
    {

        /// <summary>
        /// Program name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Program description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Program synopsis.
        /// </summary>
        public string Synopsis { get; set; }

        /// <summary>
        /// Action for unknown option.
        /// </summary>
        /// <remarks>Default value is <see cref="UnknownOptionAction.ThrowException"/></remarks>
        public UnknownOptionAction UnknownOptionAction { get; set; }

        /// <summary>
        /// Action for repeated option.
        /// </summary>
        /// <remarks>Default value is <see cref="RepeatedOptionAction.ThrowException"/></remarks>
        public RepeatedOptionAction RepeatedOptionAction { get; set; }

        /// <summary>
        /// Specification
        /// </summary>
        protected OptionSpecification Spec { get; }

        protected ProgramDescription(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentNullException(nameof(description));
            }

            Name = name;
            Description = description;
            UnknownOptionAction = UnknownOptionAction.ThrowException;
            RepeatedOptionAction = RepeatedOptionAction.ThrowException;
            Spec = new OptionSpecification();
        }

        protected ProgramDescription(string name, string description, string synopsis)
            : this(name, description)
        {
            if (string.IsNullOrWhiteSpace(synopsis))
            {
                throw new ArgumentNullException(nameof(synopsis));
            }

            Synopsis = synopsis;
        }

        protected ProgramDescription(string name, string description, string[] synopsis)
            : this(name, description, string.Join(Environment.NewLine, synopsis ?? new string[] { }))
        { }

        [ProgramFlag("ShowHelp", "-h?|--help|--show-help", "Display help information")]
        public bool ShowHelp { get; set; }

        /// <summary>
        /// Shows a help information if necessary.
        /// </summary>
        /// <returns><see cref="true"/> if help flag is present and <see cref="false"/> otherwise.</returns>
        public bool EnsureShowHelp()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parses the command line options, and configures the program description.
        /// </summary>
        /// <param name="program">ProgramDescription initial instance</param>
        /// <param name="args">Command line arguments</param>
        /// <returns><paramref name="program"/></returns>
        public static ProgramDescription Parse(ProgramDescription desc, string[] args)
        {
            if (desc == null)
            {
                throw new ArgumentNullException(nameof(desc));
            }

            // Ensure empty array for arguments
            args = args ?? new string[] { };

            /**
             * Flow:
             *  
             * 1. Extract flags/options definitions of attributes
             *    a) Check "already defined"
             * 2. Read flags/options
             *    a) Check repeated flag/option
             * 3. Check if OPTION value is valid
             * 4. Convert for arguments types
             */

            // 1. Extract flags / options definitions of attributes
            //    a) Check "already defined"
            desc.ExtractFlagOptionOfAtrributes(desc.GetType());

            // 2. Read flags / options
            //    a) Check repeated flag / option
            desc.ReadFlagsOptions(args);

            // 3. Check if OPTION value is valid
            // 4. Convert for arguments types

            return desc;
        }

        /// <summary>
        /// Extract flags and options of type atrributes
        /// </summary>
        /// <param name="typeInfo"><see cref="Type"/> instance</param>
        private void ExtractFlagOptionOfAtrributes(Type typeInfo)
        {
            foreach (var prop in typeInfo.GetProperties())
            {
                var option = prop.GetCustomAttribute(typeof(ProgramOptionAttribute));
                var flag = prop.GetCustomAttribute(typeof(ProgramFlagAttribute));

                if (option != null && flag != null)
                {
                    throw new InvalidFilterCriteriaException("You can not set a property such as OPTION and FLAG simultaneously.");
                }

                if (option != null)
                {
                    var optionDef = option as ProgramOptionAttribute;

                    if (optionDef.Define)
                    {
                        Spec.Option(optionDef.Option.Name, optionDef.Option.Pattern, optionDef.Option.Description);
                    }
                }

                if (flag != null)
                {
                    var flagDef = flag as ProgramFlagAttribute;

                    if (flagDef.Define)
                    {
                        Spec.Flag(flagDef.Flag.Name, flagDef.Flag.Pattern, flagDef.Flag.Description);
                    }
                }
            }
        }

        /// <summary>
        /// Searches for the next non-null argument and does not start with "-"
        /// </summary>
        /// <param name="start">Start index</param>
        /// <param name="args">Arguments</param>
        /// <param name="valueFound">Value found</param>
        /// <returns>Argument index, or <paramref name="start"/> if not found.</returns>
        private int GetNextValueParameter(int start, string[] args, out string valueFound)
        {
            valueFound = null;

            for (int argc = start; argc < args.Length; argc++)
            {
                string arg = args[argc];

                if (string.IsNullOrEmpty(arg)) continue;
                if (arg.First() == '-') continue;

                valueFound = arg;
                return argc;
            }

            return start;
        }

        /// <summary>
        /// Read flags and options values from arguments
        /// </summary>
        /// <param name="args">The arguments</param>
        private void ReadFlagsOptions(string[] args)
        {
            var remainingArgs = new List<string>();
            int maxArgIdx = args.Length - 1;

            for (int argc = 0; argc <= maxArgIdx; argc++)
            {
                string argOriginal = args[argc];
                string arg = argOriginal;

                // Minimal pattern: "-c"
                if (arg.Length < 2)
                {
                    remainingArgs.Add(arg);
                    continue;
                }

                char first = arg[0];
                char second = arg[1];
                bool isFullPattern = false;
                bool isShortPattern = false;

                // Full name pattern: "--full-name
                if (first == '-' && second == '-')
                {
                    arg = new string(arg.Skip(2).ToArray());
                    isFullPattern = true;
                }

                // Short name pattern: "-name"
                else if (first == '-')
                {
                    arg = new string(arg.Skip(1).ToArray());
                    isShortPattern = true;
                }

                // Invalid pattern: "?"
                if (!isFullPattern && !isShortPattern)
                {
                    remainingArgs.Add(argOriginal);
                    continue;
                }

                string argValue = null;

                // Pattern: "-x=Value", "--xx=Value"
                if (arg.Contains('='))
                {
                    var eqIdx = arg.IndexOf('=');

                    argValue = arg.Substring(eqIdx + 1);
                    arg = arg.Substring(0, eqIdx);
                }

                var argTokens = new List<string>();

                // The word is the token itself
                if (isFullPattern)
                {
                    argTokens.Add(arg);
                }

                // Each character is a token
                else if (isShortPattern)
                {
                    arg
                        .ToCharArray()
                        .ToList()
                        .ForEach(c => argTokens.Add(c.ToString()));
                }

                argTokens.ForEach(token =>
                {
                    ProgramOptionBase option = Spec.GetOptionByToken(token);

                    // Option not found!
                    if (option == null)
                    {
                        switch (UnknownOptionAction)
                        {
                            case UnknownOptionAction.ThrowException:
                                throw new UnknownOptionException(token, isFullPattern);

                            case UnknownOptionAction.ByPass:
                                if (isFullPattern)
                                {
                                    remainingArgs.Add(argOriginal);
                                }
                                else if (argValue != null)
                                {
                                    remainingArgs.Add(string.Format("-{0}={1}", token, argValue));
                                }
                                else
                                {
                                    remainingArgs.Add(string.Format("-{0}", token));
                                }
                                return; // argTokens.ForEach

                            case UnknownOptionAction.Discard:
                            default:
                                return; // argTokens.ForEach
                        }
                    }

                    // Repeated option!
                    else if (option.HasValue)
                    {
                        switch (RepeatedOptionAction)
                        {
                            case RepeatedOptionAction.ThrowException:
                                throw new RepeatedOptionException(token, isFullPattern);

                            case RepeatedOptionAction.Replace:
                                // TODO: ???
                                // Se não existe @argValue, o valor é o próximo argumento
                                // > Se não existe o próximo argumento, lança exceção de opção inválida?
                                //   - aqui podemos ou lançar a exceção,
                                //   - ou incluir uma flag que diz se é pra lanaçar a exceção
                                //   - ou simplesmente não atribuir o valor da opção
                                if (!option.IsFlag && argValue == null && argc < maxArgIdx)
                                {
                                    int nextArgIdx = GetNextValueParameter(argc + 1, args, out argValue);

                                    // Found!
                                    if (nextArgIdx != argc)
                                    {
                                        // Removes argument  found, and reconfigures context
                                        var argList = args.ToList();

                                        argList.RemoveAt(nextArgIdx);

                                        args = argList.ToArray();
                                        maxArgIdx--;
                                    }
                                }

                                if (!option.IsFlag && argValue != null)
                                {
                                    option.InformedValue = argValue;
                                }
                                return; // argTokens.ForEach

                            case RepeatedOptionAction.Ignore:
                            default:
                                return; // argTokens.ForEach
                        }
                    }

                    // Flag found for the first time
                    else if (option.IsFlag)
                    {
                        option.HasValue = true;
                        option.InformedValue = bool.TrueString;
                    }

                    // Option found for the first time
                    else
                    {
                        // TODO: ???
                        // Se não existe @argValue, o valor é o próximo argumento
                        // > Se não existe o próximo argumento, lança exceção de opção inválida?
                        //   - aqui podemos ou lançar a exceção,
                        //   - ou incluir uma flag que diz se é pra lanaçar a exceção
                        //   - ou simplesmente não atribuir o valor da opção
                        if (argValue == null && argc < maxArgIdx)
                        {
                            int nextArgIdx = GetNextValueParameter(argc + 1, args, out argValue);

                            // Found!
                            if (nextArgIdx != argc)
                            {
                                // Removes argument  found, and reconfigures context
                                var argList = args.ToList();

                                argList.RemoveAt(nextArgIdx);

                                args = argList.ToArray();
                                maxArgIdx--;
                            }
                        }

                        if (argValue != null)
                        {
                            option.HasValue = true;
                            option.InformedValue = argValue;
                        }
                    }
                });
            }
        }
    }
}
