using Lionence.VSGPT.Services.Managers;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Lionence.VSGPT.Services.Core
{
    public abstract class BaseGptService<T> : IGptService
        where T : class, new()
    {
        protected readonly IHttpClientFactory _httpClientFactory;
        protected readonly ConfigManager _configManager;

        public BaseGptService(ConfigManager configManager, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _configManager = configManager;
        }

        protected HttpClient GetHttpClient()
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configManager.ExtensionConfig.ApiKey}");
            client.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v1");
            return client;
        }

        public abstract ValueTask<T> CreateAsync(T data);
        public abstract ValueTask<T> RetrieveAsync(string id);
        public abstract ValueTask<T> ModifyAsync(T data);
        public abstract ValueTask<T> DeleteAsync(string id);
        public abstract ValueTask<ICollection<T>> ListAsync();
    }
}