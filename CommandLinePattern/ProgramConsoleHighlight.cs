using System;

namespace CommandLinePattern
{
    public class ProgramConsoleHighlight
    {
        private static ProgramConsoleHighlight _normal;
        private static ProgramConsoleHighlight _comment;
        private static ProgramConsoleHighlight _bright;

        public ConsoleColor Back { get; set; }
        public ConsoleColor Face { get; set; }

        public static ProgramConsoleHighlight Normal
        {
            get
            {
                if (_normal == null)
                {
                    _normal = new ProgramConsoleHighlight
                    {
                        Back = ConsoleColor.Black,
                        Face = ConsoleColor.White
                    };
                }

                return _normal;
            }
        }

        public static ProgramConsoleHighlight Comment
        {
            get
            {
                if (_comment == null)
                {
                    _comment = new ProgramConsoleHighlight
                    {
                        Back = ConsoleColor.Black,
                        Face = ConsoleColor.DarkGray
                    };
                }

                return _comment;
            }
        }

        public static ProgramConsoleHighlight Bright
        {
            get
            {
                if (_bright == null)
                {
                    _bright = new ProgramConsoleHighlight
                    {
                        Back = ConsoleColor.Black,
                        Face = ConsoleColor.Yellow
                    };
                }

                return _bright;
            }
        }
    }
}
