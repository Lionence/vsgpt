using Lionence.VSGPT.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lionence.VSGPT.Services
{
    internal sealed class GptFileService : BaseGptService<File>
    {
        public GptFileService(string apiKey) : base(apiKey) { }

        public override async ValueTask<File> CreateAsync(File data)
        {
            var jsonString = JsonConvert.SerializeObject(data);

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/files", new StringContent(jsonString, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<File>(responseContent);
        }

        public override async ValueTask<File> RetrieveAsync(string id)
        {
            var response = await _httpClient.GetAsync($"https://api.openai.com/v1/files/{id}");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<File>(responseContent);
        }

        public override async ValueTask<File> ModifyAsync(File data)
        {
            var jsonString = JsonConvert.SerializeObject(data);

            var response = await _httpClient.PostAsync($"https://api.openai.com/v1/files/{data.Id}", new StringContent(jsonString, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<File>(responseContent);
        }

        public override async ValueTask<File> DeleteAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"https://api.openai.com/v1/files/{id}");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<File>(responseContent);
        }

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

            var response = await _httpClient.PostAsync($"https://api.openai.com/v1/files", new StringContent(JsonConvert.SerializeObject(requestContent), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return responseContent;
        }

        public async Task<string> RetrieveFileContent(string id)
        {
            var response = await _httpClient.GetAsync($"https://api.openai.com/v1/files/{id}/content");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return responseContent;
        }
    }
}
