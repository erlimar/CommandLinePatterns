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
             * 3. Check unknown options
             * 4. Check if OPTION value is valid
             * 5. Convert for arguments types
             */

            // 1.Extract flags / options definitions of attributes
            //   a) Check "already defined"
            desc.ExtractFlagOptionOfAtrributes(desc.GetType());

            // 2.Read flags / options
            //   a) Check repeated flag / option
            desc.ReadFlagsOptions(args);

            // 3.Check unknown options
            // 4.Check if OPTION value is valid
            // 5.Convert for arguments types

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

        private void ReadFlagsOptions(string[] args)
        {
            var remainingArgs = new List<string>();

            // Output:
            // -t?
            // --test-only
            // -c
            // Comando Informado
            // --username=MyUser
            // -x=My secret =password

            for (int argc = 0; argc < args.Length; argc++)
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
                    remainingArgs.Add(arg);
                    continue;
                }

                string argValue = null;

                if (arg.Contains('='))
                {
                    var eqIdx = arg.IndexOf('=');

                    argValue = arg.Substring(eqIdx);
                    arg = arg.Substring(0, eqIdx);

                    throw new NotImplementedException("Test its!");
                }

                if (isShortPattern && argValue != null)
                {
                    throw new ArgumentException(string.Format("Invalid argument format: {0}", argOriginal));
                }

                // TODO: Se é isFullPattern, e NÃO TEM a definição, e UnknownOptionAction.ThrowException
                // -> Lança exceção de opção inválida

                // TODO: Se é isFullPattern, e NÃO TEM a definição, e UnknownOptionAction.Remove
                // -> @continue

                // TODO: Se é isFullPattern, e NÃO TEM a definição, e UnknownOptionAction.ByPass
                // -> Adiciona @argOriginal em remainingArgs e @continue

                // TODO: Se é isFullPattern, e TEM a definição, e já foi atribuído, e RepeatedOptionAction.ThrowException
                // -> Lança exceção de opção repetida

                // TODO: Se é isFullPattern, e TEM a definição, e já foi atribuído, e RepeatedOptionAction.Replace
                // -> Atribui o valor da opção
                //    # Se existe @argValue, esse é o valor
                //    # Se não existe @argValue, o valor é o próximo argumento
                //      # Se não existe o próximo argumento, lança exceção de opção inválida
                //        ? aqui podemos ou lançar a exceção, ou incluir uma flag que diz se é pra lanaçar a exceção ou simplesmente não atribuir o valor da opção

                // TODO: Se é isFullPattern, e TEM a definição, e já foi atribuído, e RepeatedOptionAction.Ignore
                // -> @continue

                // TODO: Se é isFullPattern, e TEM a definição, e NÃO foi atribuído
                // -> Atribui o valor da opção, informa que já foi atribuído
                //    # Se existe @argValue, esse é o valor
                //    # Se não existe @argValue, o valor é o próximo argumento
                //      # Se não existe o próximo argumento, lança exceção de opção inválida
                //        ? aqui podemos ou lançar a exceção, ou incluir uma flag que diz se é pra lanaçar a exceção ou simplesmente não atribuir o valor da opção

                // TODO: Se é isShortPattern. Percorre cada character
                {
                    // TODO: ...
                }
            }
        }
    }
}
