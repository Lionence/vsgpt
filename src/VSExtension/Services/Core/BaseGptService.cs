using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Lionence.VSGPT.Services.Core
{
    internal abstract class BaseGptService<T> : IGptService
        where T : class, new()
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

        public abstract ValueTask<T> CreateAsync(T data);
        public abstract ValueTask<T> RetrieveAsync(string id);
        public abstract ValueTask<T> ModifyAsync(T data);
        public abstract ValueTask<T> DeleteAsync(string id);
        public abstract ValueTask<ICollection<T>> ListAsync();
    }
}