using System.Collections.Generic;

namespace Lionence.VSGPT.Configurations
{
    public sealed class AssistantConfig
    {
        public string ProjectId { get; set; }
        public string AssistantId { get; set; }
        public string AssistantName { get; set; }
        public string ThreadId { get; set; }
        public List<string> AttachedFiles { get; set; }
    }
}
