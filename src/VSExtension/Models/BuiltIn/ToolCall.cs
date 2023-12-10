namespace Lionence.VSGPT.Models.BuiltIn
{
    public class ToolCall
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public Function Function { get; set; }
    }
}
