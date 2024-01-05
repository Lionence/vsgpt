using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Lionence.VSGPT.Models;
using Newtonsoft.Json;
using Lionence.VSGPT.Services.Core;
using Lionence.VSGPT.Services.Managers;

namespace Lionence.VSGPT.Services
{
    public sealed class GptMessageService : BaseEmbeddedGptService<Message, Thread>, IMessageService
    {
        public GptMessageService(ServiceLifetimeManager serviceLifetimeManager) : base(serviceLifetimeManager) { }


        public async ValueTask<Message> CreateAsync(MessageRequest data)
        {
            var requestContent = new
            {
                role = data.Role,
                content = data.Content,
                file_ids = data.FileIds,
                metadata = data.Metadata
            };

            var response = await TryGetHttpClient().PostAsync($"https://api.openai.com/v1/threads/{data.ThreadId}/messages", new StringContent(JsonConvert.SerializeObject(requestContent), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return JsonConvert.DeserializeObject<Message>(responseContent);
        }

        public override async ValueTask<Message> RetrieveAsync(string threadId, string messageId)
        {
            var response = await TryGetHttpClient().GetAsync($"https://api.openai.com/v1/threads/{threadId}/messages/${messageId}");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return JsonConvert.DeserializeObject<Message>(responseContent);
        }

        public override async ValueTask<ICollection<Message>> ListAsync(string threadId)
        {
            var response = await TryGetHttpClient().GetAsync($"https://api.openai.com/v1/threads/{threadId}/messages");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return JsonConvert.DeserializeObject<ICollection<Message>>(responseContent);
        }

        public override async ValueTask<Message> ModifyAsync(Message data)
        {
            var requestContent = new
            {
                metadata = data.Metadata
            };

            var response = await TryGetHttpClient().PostAsync($"https://api.openai.com/v1/threads/{data.ThreadId}/messages/{data.Id}", new StringContent(JsonConvert.SerializeObject(requestContent), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Message>(responseContent);
        }

        public override async ValueTask<Message> DeleteAsync(string id)
        {
            var response = await TryGetHttpClient().DeleteAsync($"https://api.openai.com/v1/threads/{id}");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return JsonConvert.DeserializeObject<Message>(responseContent);
        }

        public override ValueTask<Message> CreateAsync(Message data)
            => throw new NotSupportedException("Use CreateAsync(MessageRequest data) instead!");
    }
}
