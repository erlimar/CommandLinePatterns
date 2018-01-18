namespace CommandLinePattern
{
    public interface IProgramConsole
    {
        void Say(string message, ProgramConsoleHighlight highlight = null);
    }
}
