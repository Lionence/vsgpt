using Lionence.VSGPT.Models.BuiltIn;

namespace Lionence.VSGPT.Models
{
    public sealed class MessageContent
    {
        public string Type { get; set; }
        public TextContent Text { get; set; }
    }
}
