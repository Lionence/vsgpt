using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lionence.VSGPT.Services
{
    public class GptThreadService : BaseGptService
    {
        public GptThreadService(string apiKey) : base(apiKey) { }

        public async Task<string> CreateThread()
        {
            var response = await _httpClient.PostAsync("https://api.openai.com/v1/threads", new StringContent("", Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return responseContent;
        }

        public async Task<string> RetrieveThread(string threadId)
        {
            var response = await _httpClient.GetAsync($"https://api.openai.com/v1/threads/{threadId}");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return responseContent;
        }

        public async Task<string> ModifyThread(string threadId, string metadata)
        {
            var requestContent = new
            {
                metadata
            };

            var response = await _httpClient.PostAsync($"https://api.openai.com/v1/threads/{threadId}", new StringContent(requestContent.ToString(), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return responseContent;
        }

        public async Task<string> DeleteThread(string threadId)
        {
            var response = await _httpClient.DeleteAsync($"https://api.openai.com/v1/threads/{threadId}");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return responseContent;
        }
    }
}
