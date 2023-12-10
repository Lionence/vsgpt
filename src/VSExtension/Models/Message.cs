using System.Collections.Generic;

namespace Lionence.VSGPT.Models
{
    internal sealed class Message
    {
        public string Id { get; set; }
        public long CreatedAt { get; set; }
        public string ThreadId { get; set; }
        public string Role { get; set; }
        public List<MessageContent> Content { get; set; }
        public string AssistantId { get; set; }
        public string RunId { get; set; }
        public List<string> FileIds { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}
