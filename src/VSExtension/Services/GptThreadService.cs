﻿using Lionence.VSGPT.Models;
using Lionence.VSGPT.Services.Core;
using Lionence.VSGPT.Services.Managers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lionence.VSGPT.Services
{
    public sealed class GptThreadService : BaseGptService<Thread>
    {
        public GptThreadService(ServiceLifetimeManager serviceLifetimeManager) : base(serviceLifetimeManager) { }

        public override async ValueTask<Thread> CreateAsync(Thread data)
        {
            var response = await GetHttpClient().PostAsync("https://api.openai.com/v1/threads", new StringContent("", Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return JsonConvert.DeserializeObject<Thread>(responseContent);
        }

        public override async ValueTask<Thread> RetrieveAsync(string projectGuid)
            => (await ListAsync()).SingleOrDefault(
                a => a.Metadata.ContainsKey("purpose") && a.Metadata["purpose"] == "vsgpt"
                  && a.Metadata.ContainsKey("project") && a.Metadata["project"] == projectGuid);

        public override async ValueTask<ICollection<Thread>> ListAsync()
        {
            var response = await GetHttpClient().GetAsync("https://api.openai.com/v1/threads/");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return JsonConvert.DeserializeObject<ICollection<Thread>>(responseContent);
        }

        public override async ValueTask<Thread> ModifyAsync(Thread data)
        {
            var requestContent = new
            {
                metadata = data.Metadata
            };

            var response = await GetHttpClient().PostAsync($"https://api.openai.com/v1/threads/{data.Id}", new StringContent(JsonConvert.SerializeObject(requestContent), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return JsonConvert.DeserializeObject<Thread>(responseContent);
        }

        public override async ValueTask<Thread> DeleteAsync(string id)
        {
            var response = await GetHttpClient().DeleteAsync($"https://api.openai.com/v1/threads/{id}");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            // Parse the response content if needed
            return JsonConvert.DeserializeObject<Thread>(responseContent);
        }
    }
}
