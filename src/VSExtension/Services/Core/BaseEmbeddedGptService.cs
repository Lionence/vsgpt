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
        protected readonly HttpClient _httpClient;
        protected readonly ConfigManager _configManager;

        public BaseEmbeddedGptService(ConfigManager configManager)
        {
            _httpClient = new HttpClient();
            _configManager = configManager;
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configManager}");
            _httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v1");
        }

        public abstract ValueTask<TObject> CreateAsync(TObject data);
        public abstract ValueTask<TObject> RetrieveAsync(string parentId, string objectId);
        public abstract ValueTask<TObject> ModifyAsync(TObject data);
        public abstract ValueTask<TObject> DeleteAsync(string objectId);
        public abstract ValueTask<ICollection<TObject>> ListAsync(string parentId);
    }
}