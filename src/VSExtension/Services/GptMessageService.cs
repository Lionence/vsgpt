using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Lionence.VSGPT.Models;
using Newtonsoft.Json;

namespace Lionence.VSGPT.Services
{
    internal sealed class GptMessageService : BaseGptService<Message>
    {
        public GptMessageService(string apiKey) : base(apiKey) { }

        public override async ValueTask<Message> CreateAsync(Message data)
        {
            var requestContent = new
            {
                role = data.Role,
                content = data.Content,
                file_ids = data.FileIds,
                metadata = data.Metadata
            };

            var response = await _httpClient.PostAsync($"https://api.openai.com/v1/threads/{data.ThreadId}/messages", new StringContent(JsonConvert.SerializeObject(requestContent), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return JsonConvert.DeserializeObject<Message>(responseContent);
        }

        public override async ValueTask<Message> RetrieveAsync(string id)
        {
            var response = await _httpClient.GetAsync($"https://api.openai.com/v1/threads/{id}");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return JsonConvert.DeserializeObject<Message>(responseContent);
        }

        public override async ValueTask<Message> ModifyAsync(Message data)
        {
            var requestContent = new
            {
                metadata = data.Metadata
            };

            var response = await _httpClient.PostAsync($"https://api.openai.com/v1/threads/{data.ThreadId}/messages/{data.Id}", new StringContent(JsonConvert.SerializeObject(requestContent), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Message>(responseContent);
        }

        public override async ValueTask<Message> DeleteAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"https://api.openai.com/v1/threads/{id}");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return JsonConvert.DeserializeObject<Message>(responseContent);
        }
    }
}
