using Lionence.VSGPT.Models;
using System.Threading.Tasks;

namespace Lionence.VSGPT.Services.Core
{
    internal interface IMessageService
    {
        ValueTask<Message> CreateAsync(MessageRequest data);
    }
}
