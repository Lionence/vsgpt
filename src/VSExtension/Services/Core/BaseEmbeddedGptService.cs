using Lionence.VSGPT.Services.Managers;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Lionence.VSGPT.Services.Core
{
    public abstract class BaseEmbeddedGptService<TObject, TParent> : IGptService
        where TObject : class, new()
        where TParent : class, new()
    {
        protected readonly IHttpClientFactory _httpClientFactory;
        protected readonly ConfigManager _configManager;

        public BaseEmbeddedGptService(ConfigManager configManager, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _configManager = configManager;
        }

        protected HttpClient TryGetHttpClient()
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configManager.ExtensionConfig.ApiKey}");
            client.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v1");
            return client;
        }

        public abstract ValueTask<TObject> CreateAsync(TObject data);
        public abstract ValueTask<TObject> RetrieveAsync(string parentId, string objectId);
        public abstract ValueTask<TObject> ModifyAsync(TObject data);
        public abstract ValueTask<TObject> DeleteAsync(string objectId);
        public abstract ValueTask<ICollection<TObject>> ListAsync(string parentId);
    }
}