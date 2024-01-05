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
        private readonly ServiceLifetimeManager _serviceLifetimeManage;

        private readonly IHttpClientFactory _httpClientFactory;

        protected ConfigManager ConfigManager => _serviceLifetimeManage.Get<ConfigManager>();

        public BaseEmbeddedGptService(ServiceLifetimeManager serviceLifetimeManager)
        {
            _serviceLifetimeManage = serviceLifetimeManager;
            _httpClientFactory = serviceLifetimeManager.Get<IHttpClientFactory>();
        }

        protected HttpClient TryGetHttpClient()
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ConfigManager.ExtensionConfig.ApiKey}");
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