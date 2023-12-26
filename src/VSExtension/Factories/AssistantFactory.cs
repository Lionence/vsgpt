using Lionence.VSGPT.Models;

namespace Lionence.VSGPT.Factories
{
    public class AssistantFactory
    {
        public AssistantFactory() { }

        public Assistant CreateAssistant()
        {
            return new Assistant();
        }
    }
}
