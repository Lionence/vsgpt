using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lionence.VSGPT.Services
{
    public class GptFileService : BaseGptService
    {
        public GptFileService(string apiKey) : base(apiKey) { }

        public async Task<string> ListFiles(string purpose = null)
        {
            var queryParameters = purpose != null ? $"?purpose={purpose}" : "";
            var response = await _httpClient.GetAsync($"https://api.openai.com/v1/files{queryParameters}");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return responseContent;
        }

        public async Task<string> UploadFile(string purpose, string fileContent)
        {
            var requestContent = new
            {
                purpose,
                file = fileContent
            };

            var response = await _httpClient.PostAsync($"https://api.openai.com/v1/files", new StringContent(requestContent.ToString(), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return responseContent;
        }

        public async Task<string> DeleteFile(string fileId)
        {
            var response = await _httpClient.DeleteAsync($"https://api.openai.com/v1/files/{fileId}");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return responseContent;
        }

        public async Task<string> RetrieveFile(string fileId)
        {
            var response = await _httpClient.GetAsync($"https://api.openai.com/v1/files/{fileId}");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return responseContent;
        }

        public async Task<string> RetrieveFileContent(string fileId)
        {
            var response = await _httpClient.GetAsync($"https://api.openai.com/v1/files/{fileId}/content");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return responseContent;
        }
    }
}
