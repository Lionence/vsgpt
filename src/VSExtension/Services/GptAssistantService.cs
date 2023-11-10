using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lionence.VSGPT.Services
{
    public class GptAssistantService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey; // Your OpenAI API key

        public GptAssistantService(string apiKey)
        {
            _httpClient = new HttpClient();
            _apiKey = apiKey;
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            _httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v1");
        }

        public async Task<string> CreateAssistant(string model, string name, string description, string instructions, string[] tools)
        {
            var requestContent = new
            {
                model,
                name,
                description,
                instructions,
                tools
            };

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/assistants", new StringContent(requestContent.ToString(), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return responseContent;
        }

        public async Task<string> RetrieveAssistant(string assistantId)
        {
            var response = await _httpClient.GetAsync($"https://api.openai.com/v1/assistants/{assistantId}");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return responseContent;
        }

        public async Task<string> ModifyAssistant(string assistantId, string model, string name, string description, string instructions, string[] tools)
        {
            var requestContent = new
            {
                model,
                name,
                description,
                instructions,
                tools
            };

            var response = await _httpClient.PostAsync($"https://api.openai.com/v1/assistants/{assistantId}", new StringContent(requestContent.ToString(), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return responseContent;
        }

        public async Task<string> DeleteAssistant(string assistantId)
        {
            var response = await _httpClient.DeleteAsync($"https://api.openai.com/v1/assistants/{assistantId}");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return responseContent;
        }
    }
}
