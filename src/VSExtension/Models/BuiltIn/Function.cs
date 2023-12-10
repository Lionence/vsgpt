namespace Lionence.VSGPT.Models.BuiltIn
{
    public class Function
    {
        public string Name { get; set; }
        public string Arguments { get; set; }
        public string Description { get; set; }
        public object Parameters { get; set; } // You might need to specify a specific type for Parameters based on the JSON Schema
    }
}
