using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Lionence.VSGPT.Models;
using System.Collections.Generic;
using System;
using Lionence.VSGPT.Services.Core;
using Lionence.VSGPT.Services.Managers;

namespace Lionence.VSGPT.Services
{
    public sealed class GptRunService : BaseEmbeddedGptService<Run, Thread>
    {
        public GptRunService(ServiceLifetimeManager serviceLifetimeManager) : base(serviceLifetimeManager) { }

        public override async ValueTask<Run> CreateAsync(Run data)
        {
            var jsonString = JsonConvert.SerializeObject(data);

            var response = await TryGetHttpClient().PostAsync($"https://api.openai.com/v1/threads/{data.ThreadId}/runs", new StringContent(jsonString, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Run>(responseContent);
        }

        public override async ValueTask<Run> RetrieveAsync(string threadId, string runId)
        {
            var response = await TryGetHttpClient().GetAsync($"https://api.openai.com/v1/threads/{threadId}/runs/${runId}");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Run>(responseContent);
        }

        public override async ValueTask<Run> ModifyAsync(Run data)
        {
            var jsonString = JsonConvert.SerializeObject(data);

            var response = await TryGetHttpClient().PostAsync($"https://api.openai.com/v1/threads/{data.ThreadId}/runs", new StringContent(jsonString, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Run>(responseContent);
        }

        public override async ValueTask<ICollection<Run>> ListAsync(string threadId)
        {
            var response = await TryGetHttpClient().GetAsync($"https://api.openai.com/v1/threads/${threadId}/runs");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<Run>>(responseContent);
        }

        public override ValueTask<Run> DeleteAsync(string id)
            => throw new NotSupportedException();
    }
}
