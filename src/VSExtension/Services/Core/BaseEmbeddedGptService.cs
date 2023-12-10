using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Lionence.VSGPT.Services.Core
{
    internal abstract class BaseEmbeddedGptService<TObject, TParent> : IGptService
        where TObject : class, new()
        where TParent : class, new()
    {
        protected readonly HttpClient _httpClient;
        protected readonly string _apiKey;

        public BaseEmbeddedGptService(string apiKey)
        {
            _httpClient = new HttpClient();
            _apiKey = apiKey;
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            _httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v1");
        }

        public abstract ValueTask<TObject> CreateAsync(TObject data);
        public abstract ValueTask<TObject> RetrieveAsync(string parentId, string objectId);
        public abstract ValueTask<TObject> ModifyAsync(TObject data);
        public abstract ValueTask<TObject> DeleteAsync(string objectId);
        public abstract ValueTask<ICollection<TObject>> ListAsync(string parentId);
    }
}