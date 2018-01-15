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
            desc.ExtractFlagOptionOfAtrributes(typeof(ProgramDescription).GetTypeInfo());

            if (desc.GetType() != typeof(ProgramDescription))
            {
                desc.ExtractFlagOptionOfAtrributes(desc.GetType().GetTypeInfo());
            }

            // 2.Read flags / options
            //   a) Check repeated flag / option
            // 3.Check unknown options
            // 4.Check if OPTION value is valid
            // 5.Convert for arguments types

            return desc;
        }

        private void ExtractFlagOptionOfAtrributes(TypeInfo typeInfo)
        {
            foreach (var prop in typeInfo.DeclaredProperties)
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
    }
}
