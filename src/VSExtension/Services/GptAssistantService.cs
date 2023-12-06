using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Lionence.VSGPT.Models;

namespace Lionence.VSGPT.Services
{
    internal sealed class GptAssistantService : BaseGptService<Assistant>
    {
        public GptAssistantService(string apiKey) : base(apiKey) { }

        public override async ValueTask<Assistant> CreateAsync(Assistant data)
        {
            var jsonString = JsonConvert.SerializeObject(data);

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/assistants", new StringContent(jsonString, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Assistant>(responseContent);
        }

        public override async ValueTask<Assistant> RetrieveAsync(string id)
        {
            var response = await _httpClient.GetAsync($"https://api.openai.com/v1/assistants/{id}");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Assistant>(responseContent);
        }

        public override async ValueTask<Assistant> ModifyAsync(Assistant data)
        {
            var jsonString = JsonConvert.SerializeObject(data);

            var response = await _httpClient.PostAsync($"https://api.openai.com/v1/assistants/{data.Id}", new StringContent(jsonString, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Assistant>(responseContent);
        }

        public override async ValueTask<Assistant> DeleteAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"https://api.openai.com/v1/assistants/{id}");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Assistant>(responseContent);
        }
    }
}
