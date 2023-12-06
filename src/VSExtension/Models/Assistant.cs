using System.Collections.Generic;

namespace Lionence.VSGPT.Models
{
    internal sealed class Assistant
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public long CreatedAt { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Model { get; set; }
        public string Instructions { get; set; }
        public List<string> Tools { get; set; }
        public List<string> FileIds { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}