using Lionence.VSGPT.Models;
using Lionence.VSGPT.Services.Core;
using Lionence.VSGPT.Services.Managers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lionence.VSGPT.Services
{
    public sealed class GptFileService : BaseGptService<File>
    {
        public GptFileService(ConfigManager configManager) : base(configManager) { }

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
            // Parse the response content if needed
            return JsonConvert.DeserializeObject<File>(responseContent);
        }

        public override async ValueTask<ICollection<File>> ListAsync()
        {
            var response = await _httpClient.GetAsync("https://api.openai.com/v1/files/");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return JsonConvert.DeserializeObject<ICollection<File>>(responseContent);
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

        public async Task<string> UploadFileAsync(string purpose, string fileContent)
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

        public async Task<string> RetrieveFileContentAsync(string id)
        {
            var response = await _httpClient.GetAsync($"https://api.openai.com/v1/files/{id}/content");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return responseContent;
        }
    }
}
