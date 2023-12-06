using System.Collections.Generic;

namespace Lionence.VSGPT.Models
{
    internal sealed class TextContent
    {
        public string Value { get; set; }
        public List<string> Annotations { get; set; }
    }
}
