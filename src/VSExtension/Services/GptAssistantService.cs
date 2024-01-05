using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Lionence.VSGPT.Models;
using System.Collections.Generic;
using Lionence.VSGPT.Services.Core;
using Lionence.VSGPT.Services.Managers;

namespace Lionence.VSGPT.Services
{
    public sealed class GptAssistantService : BaseGptService<Assistant>
    {
        public GptAssistantService(ServiceLifetimeManager serviceLifetimeManager) : base(serviceLifetimeManager) { }

        public override async ValueTask<Assistant> CreateAsync(Assistant data)
        {
            var jsonString = JsonConvert.SerializeObject(data);

            var response = await GetHttpClient().PostAsync("https://api.openai.com/v1/assistants", new StringContent(jsonString, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Assistant>(responseContent);
        }

        public override async ValueTask<Assistant> RetrieveAsync(string assistantId)
        {
            var response = await GetHttpClient().GetAsync($"https://api.openai.com/v1/assistants/{assistantId}");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Assistant>(responseContent);
        }

        public override async ValueTask<Assistant> ModifyAsync(Assistant data)
        {
            var jsonString = JsonConvert.SerializeObject(data);

            var response = await GetHttpClient().PostAsync($"https://api.openai.com/v1/assistants/{data.Id}", new StringContent(jsonString, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Assistant>(responseContent);
        }

        public override async ValueTask<Assistant> DeleteAsync(string id)
        {
            var response = await GetHttpClient().DeleteAsync($"https://api.openai.com/v1/assistants/{id}");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Assistant>(responseContent);
        }

        public override async ValueTask<ICollection<Assistant>> ListAsync()
        {
            var response = await GetHttpClient().GetAsync($"https://api.openai.com/v1/assistants/");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<Assistant>>(responseContent);
        }
    }
}
