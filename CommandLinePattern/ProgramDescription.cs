using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace CommandLinePattern
{
    public class ProgramDescription
    {
        private readonly IProgramConsole _console;

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
        /// Program arguments
        /// </summary>
        public string[] Arguments { get; private set; }

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
        /// Command line wrapper for show help information flag.
        /// </summary>
        [ProgramFlag("ShowHelp", "-h?|--help|--show-help", "Display help information")]
        public bool ShowHelp { get; protected set; }

        /// <summary>
        /// Specification
        /// </summary>
        protected OptionSpecification Spec { get; }

        protected ProgramDescription(IProgramConsole console, string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentNullException(nameof(description));
            }

            _console = console;
            Name = name;
            Description = description;
            UnknownOptionAction = UnknownOptionAction.ThrowException;
            RepeatedOptionAction = RepeatedOptionAction.ThrowException;
            Spec = new OptionSpecification();
        }

        protected ProgramDescription(IProgramConsole console, string name, string description, string synopsis)
            : this(console, name, description)
        {
            if (string.IsNullOrWhiteSpace(synopsis))
            {
                throw new ArgumentNullException(nameof(synopsis));
            }

            Synopsis = synopsis;
        }

        protected ProgramDescription(IProgramConsole console, string name, string description, string[] synopsis)
            : this(console, name, description, string.Join(Environment.NewLine, synopsis ?? new string[] { }))
        { }

        /// <summary>
        /// Shows a help information if necessary.
        /// </summary>
        /// <returns><see cref="true"/> if help flag is present and <see cref="false"/> otherwise.</returns>
        public bool EnsureShowHelp()
        {
            if (ShowHelp)
            {
                SayFullLogo();
                SayUsage();

                int columnWidth = CalculateMaxWidthForOptionFlagExpression();

                SayOptions(columnWidth);
                SayFlags(columnWidth);
            }

            return ShowHelp;
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

            desc.ExtractFlagOptionOfAttributes();
            desc.ReadFlagsOptions(args);
            desc.ValidateOptionsAcceptedValues();
            desc.PopulateOptionsAttributes();

            return desc;
        }

        /// <summary>
        /// Validate option accepted values.
        /// </summary>
        private void ValidateOptionsAcceptedValues()
        {
            Spec.GetAllOptions()
                .Where(w => w.HasValue && !w.IsFlag && w.AcceptedValues.Count > 0)
                .ToList()
                .ForEach(option =>
                {
                    if (!option.AcceptedValues.Any(a => string.Compare(a.Value, option.InformedValue, true) == 0))
                    {
                        throw new InvalidOptionValueException(option.Name, option.InformedValue);
                    }
                });
        }

        /// <summary>
        /// Populate object options attributes
        /// </summary>
        private void PopulateOptionsAttributes()
        {
            foreach (var prop in GetType().GetProperties())
            {
                var optionAttr = prop.GetCustomAttribute(typeof(OptionAttribute)) as OptionAttribute;

                if (optionAttr != null)
                {
                    ProgramOptionBase option = Spec.GetOptionByName(optionAttr.Name);

                    if (option == null)
                    {
                        continue;
                    }

                    object optionValue = option.ConvertValueToType(prop.PropertyType);

                    if (optionValue != null)
                    {
                        prop.SetValue(this, optionValue);
                    }
                }
            }
        }

        /// <summary>
        /// Extract flags and options of type atrributes
        /// </summary>
        private void ExtractFlagOptionOfAttributes()
        {
            foreach (var prop in GetType().GetProperties())
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
                        Spec.Option(optionDef.Name, optionDef.Pattern, optionDef.Description);
                    }

                    // TODO: Lazy add specification
                    var optionSpec = Spec.GetOptionByName(optionDef.Name);

                    if (optionSpec != null)
                    {
                        ExtractOptionValuesOfAttributes(prop, optionSpec);
                    }
                }

                if (flag != null)
                {
                    var flagDef = flag as ProgramFlagAttribute;

                    if (flagDef.Define)
                    {
                        Spec.Flag(flagDef.Name, flagDef.Pattern, flagDef.Description);
                    }
                }
            }
        }

        /// <summary>
        /// Extract option accepted values from object attributes.
        /// </summary>
        /// <param name="prop">Property object</param>
        /// <param name="optionSpec">Option specification object</param>
        private void ExtractOptionValuesOfAttributes(PropertyInfo prop, ProgramOptionBase optionSpec)
        {
            /// <remarks>
            /// <see cref="ProgramOptionValueAttribute"/> is <see cref="ProgramOptionValuesAttribute"/>. Therefore, listing <see cref="ProgramOptionValueAttribute"/> already included.
            /// </remarks>
            IEnumerable<Attribute> valuesAttrList = prop.GetCustomAttributes(typeof(ProgramOptionValuesAttribute));

            if (valuesAttrList == null) return;

            valuesAttrList.ToList().ForEach(v =>
            {
                IEnumerable<ProgramAcceptedValue> acceptedValues = (v as ProgramOptionValuesAttribute).Values.Select(s =>
                {
                    string[] values = s.Split('|');

                    if (values.Length < 1) return null;

                    var value = new ProgramAcceptedValue
                    {
                        Value = values.First().Trim()
                    };

                    if (values.Length > 1)
                    {
                        value.Description = values.Skip(1).First().Trim();
                    }

                    return value;

                }).Where(w => w != null);

                optionSpec.AcceptedValues.AddRange(acceptedValues);
            });
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

            Arguments = remainingArgs.ToArray();
        }

        private void SayFullLogo()
        {
            SayLogo();

            if (!string.IsNullOrWhiteSpace(Synopsis))
            {
                _console.Say(Synopsis, ProgramConsoleHighlight.Comment);
                _console.Say(null);
            }
        }

        private void SayLogo()
        {
            string programName = Name;

            if (!string.IsNullOrWhiteSpace(Description))
            {
                programName += string.Format(" - {0}", Description);
            }

            _console.Say(programName);
            _console.Say(null);
        }

        private void SayUsage()
        {
            string prompt = string.Format("{0}$ {1} [option|flag] [args]", Tab(1), Name);

            _console.Say("USAGE:", ProgramConsoleHighlight.Bright);
            _console.Say(null);

            _console.Say(prompt);
            _console.Say(null);
        }

        private void SayOptions(int columnWidth)
        {
            var options = Spec.GetAllOptions().Where(o => !o.IsFlag);

            if (!options.Any())
            {
                return;
            }

            _console.Say("OPTIONS:", ProgramConsoleHighlight.Bright);
            _console.Say(null);

            foreach (var option in options)
            {
                string optionExp = string.Format("{0}{1}=<{2}>", Tab(1), option.Pattern, option.Name);
                string optionLine = string.Format("{0} {1}", optionExp.PadRight(columnWidth), option.Description);

                _console.Say(optionLine);

                if (option.AcceptedValues.Any())
                {
                    _console.Say(string.Format("{0} Accepted values:", Tab(2)), ProgramConsoleHighlight.Comment);

                    option.AcceptedValues.ForEach(v =>
                    {
                        string valueExp = string.Format("{0} - {1}", Tab(2), v.Value);
                        string valueLine = string.Format("{0} {1}", valueExp.PadRight(columnWidth), v.Description);

                        _console.Say(valueLine, ProgramConsoleHighlight.Comment);
                    });
                }
            }

            _console.Say(null);
        }

        private void SayFlags(int columnWidth)
        {
            var flags = Spec.GetAllOptions().Where(o => o.IsFlag);

            if (!flags.Any())
            {
                return;
            }

            _console.Say("FLAGS:", ProgramConsoleHighlight.Bright);
            _console.Say(null);

            foreach (var flag in flags)
            {
                string flagExp = string.Format("{0}{1}", Tab(1), flag.Pattern, flag.Name);
                string flagLine = string.Format("{0} {1}", flagExp.PadRight(columnWidth), flag.Description);

                _console.Say(flagLine);
            }

            _console.Say(null);
        }

        private string Tab(int count)
        {
            return new string(' ', count * 4);
        }

        private int CalculateMaxWidthForOptionFlagExpression()
        {
            // TODO: Calculate
            return 40;
        }
    }
}
