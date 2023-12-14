using System.Collections.Generic;

namespace Lionence.VSGPT.Models
{
    public class MessageRequest
    {
        public string ThreadId { get; set; }
        public string Role { get; set; }
        public string Content { get; set; }
        public List<string> FileIds { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}
