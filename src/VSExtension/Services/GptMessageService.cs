using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lionence.VSGPT.Services
{
    public class GptMessageService : BaseGptService
    {
        public GptMessageService(string apiKey) : base(apiKey) { }

        public async Task<string> CreateMessage(string threadId, string role, string content, string[] fileIds = null, string metadata = null)
        {
            var requestContent = new
            {
                role,
                content,
                file_ids = fileIds ?? new string[0],
                metadata
            };

            var response = await _httpClient.PostAsync($"https://api.openai.com/v1/threads/{threadId}/messages", new StringContent(requestContent.ToString(), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return responseContent;
        }

        public async Task<string> RetrieveMessage(string threadId, string messageId)
        {
            var response = await _httpClient.GetAsync($"https://api.openai.com/v1/threads/{threadId}/messages/{messageId}");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return responseContent;
        }

        public async Task<string> ModifyMessage(string threadId, string messageId, string metadata)
        {
            var requestContent = new
            {
                metadata
            };

            var response = await _httpClient.PostAsync($"https://api.openai.com/v1/threads/{threadId}/messages/{messageId}", new StringContent(requestContent.ToString(), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return responseContent;
        }

        public async Task<string> ListMessages(string threadId, int limit = 20, string order = "desc", string after = null, string before = null)
        {
            var response = await _httpClient.GetAsync($"https://api.openai.com/v1/threads/{threadId}/messages?limit={limit}&order={order}&after={after}&before={before}");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return responseContent;
        }

        public async Task<string> RetrieveMessageFile(string threadId, string messageId, string fileId)
        {
            var response = await _httpClient.GetAsync($"https://api.openai.com/v1/threads/{threadId}/messages/{messageId}/files/{fileId}");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return responseContent;
        }

        public async Task<string> ListMessageFiles(string threadId, string messageId, int limit = 20, string order = "desc", string after = null, string before = null)
        {
            var response = await _httpClient.GetAsync($"https://api.openai.com/v1/threads/{threadId}/messages/{messageId}/files?limit={limit}&order={order}&after={after}&before={before}");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return responseContent;
        }
    }
}
