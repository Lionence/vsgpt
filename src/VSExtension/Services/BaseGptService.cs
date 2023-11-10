using System.Net.Http;

namespace Lionence.VSGPT.Services
{
    public abstract class BaseGptService
    {
        protected readonly HttpClient _httpClient;
        protected readonly string _apiKey;

        public BaseGptService(string apiKey)
        {
            _httpClient = new HttpClient();
            _apiKey = apiKey;
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            _httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v1");
        }
    }
}