using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLinePattern
{
    public class OptionSpecification
    {
        private readonly Dictionary<string, ProgramOptionBase> _specByToken
            = new Dictionary<string, ProgramOptionBase>();

        private readonly Dictionary<string, ProgramOptionBase> _specByName
            = new Dictionary<string, ProgramOptionBase>();

        public ProgramOptionBase GetOptionByToken(string token)
        {
            if (!_specByToken.ContainsKey(token))
            {
                return null;
            }

            return _specByToken[token];
        }

        public ProgramOptionBase GetOptionByName(string name)
        {
            if (!_specByName.ContainsKey(name))
            {
                return null;
            }

            return _specByName[name];
        }

        public OptionSpecification Option(string name, string pattern, string description)
        {
            AddSpecification(name, new ProgramOptionBase
            {
                Name = name,
                Pattern = pattern,
                Description = description,
                IsFlag = false
            });

            return this;
        }

        public OptionSpecification Flag(string name, string pattern, string description)
        {
            AddSpecification(name, new ProgramOptionBase
            {
                Name = name,
                Pattern = pattern,
                Description = description,
                IsFlag = true
            });

            return this;
        }

        private void AddSpecification(string name, ProgramOptionBase option)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            // TODO: Patter is required?
            if (string.IsNullOrWhiteSpace(option.Pattern))
            {
                throw new ArgumentNullException(nameof(option.Pattern).ToLower());
            }

            // TODO: Description is required?
            if (string.IsNullOrWhiteSpace(option.Description))
            {
                throw new ArgumentNullException(nameof(option.Description).ToLower());
            }

            // Duplicate key exception by Dictionary
            _specByName.Add(name, option);

            // Duplicate key exception by Dictionary
            option.Pattern.Split('|').ToList().ForEach(keyword =>
            {
                var argumentException = new ArgumentException(string.Format("Invalid keyword pattern: {0}", keyword));

                // Minimal pattern: "-c"
                if (keyword.Length < 2)
                {
                    throw argumentException;
                }

                char first = keyword[0];
                char second = keyword[1];
                bool isFullPattern = true;

                // Full name pattern: "--full-name
                if (first == '-' && second == '-')
                {
                    keyword = new string(keyword.Skip(2).ToArray());
                }

                // Short name pattern: "-name"
                else if (first == '-')
                {
                    keyword = new string(keyword.Skip(1).ToArray());
                    isFullPattern = false;
                }

                // Invalid pattern: "?"
                else
                {
                    throw argumentException;
                }

                // Invalid pattern: "--", "-\s", "- "
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    throw argumentException;
                }

                if (isFullPattern)
                {
                    _specByToken.Add(keyword, option);
                }
                else
                {
                    keyword
                        .ToCharArray()
                        .ToList()
                        .ForEach(c => _specByToken.Add(c.ToString(), option));
                }
            });
        }
    }
}
