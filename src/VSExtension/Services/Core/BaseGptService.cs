using Lionence.VSGPT.Services.Managers;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Lionence.VSGPT.Services.Core
{
    public abstract class BaseGptService<T> : IGptService
        where T : class, new()
    {
        protected readonly HttpClient _httpClient;
        protected readonly ConfigManager _configManager;

        public BaseGptService(ConfigManager configManager)
        {
            _httpClient = new HttpClient();
            _configManager = configManager;
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configManager.ExtensionConfig.ApiKey}");
            _httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v1");
        }

        public abstract ValueTask<T> CreateAsync(T data);
        public abstract ValueTask<T> RetrieveAsync(string id);
        public abstract ValueTask<T> ModifyAsync(T data);
        public abstract ValueTask<T> DeleteAsync(string id);
        public abstract ValueTask<ICollection<T>> ListAsync();
    }
}