using System.Collections.Generic;

namespace Lionence.VSGPT.Models
{
    internal sealed class Thread
    {
        public string Id { get; set; }
        public long CreatedAt { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}
