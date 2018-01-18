using System;

namespace CommandLinePattern
{
    public class ProgramConsole : IProgramConsole
    {
        public void Say(string message, ProgramConsoleHighlight highlight = null)
        {
            highlight = highlight ?? ProgramConsoleHighlight.Normal;

            Console.BackgroundColor = highlight.Back;
            Console.ForegroundColor = highlight.Face;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
