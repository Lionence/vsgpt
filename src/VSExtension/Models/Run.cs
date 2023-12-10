using Lionence.VSGPT.Models.BuiltIn;
using System.Collections.Generic;

namespace Lionence.VSGPT.Models
{
    public sealed class Run
    {
        public string Id { get; set; }
        public long CreatedAt { get; set; }
        public string ThreadId { get; set; }
        public string AssistantId { get; set; }
        public string Status { get; set; }
        public RequiredAction RequiredAction { get; set; }
        public long ExpiresAt { get; set; }
        public long? StartedAt { get; set; }
        public long? CancelledAt { get; set; }
        public long? FailedAt { get; set; }
        public long? CompletedAt { get; set; }
        public string Model { get; set; }
        public string Instructions { get; set; }
        public List<ToolCall> Tools { get; set; }
        public List<string> FileIds { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}
