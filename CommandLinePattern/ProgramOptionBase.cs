namespace CommandLinePattern
{
    public class ProgramOptionBase
    {
        public string Name { get; set; }
        public string Pattern { get; set; }
        public string Description { get; set; }
        public bool IsFlag { get; set; }
        public string InformedValue { get; set; }
        public object CalculatedValue { get; set; }
    }
}
