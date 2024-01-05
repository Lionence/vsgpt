using Lionence.VSGPT.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lionence.VSGPT.Services.Managers
{
    public sealed class WorkflowManager
    {
        private readonly ServiceLifetimeManager _serviceLifetimeManager;

        private ConfigManager ConfigManager => _serviceLifetimeManager.Get<ConfigManager>();
        private GptAssistantService AssistantService => _serviceLifetimeManager.Get<GptAssistantService>();
        private GptThreadService ThreadService => _serviceLifetimeManager.Get<GptThreadService>();
        private GptRunService RunService => _serviceLifetimeManager.Get<GptRunService>();
        private GptMessageService MessageService => _serviceLifetimeManager.Get<GptMessageService>();

        public WorkflowManager(ServiceLifetimeManager serviceLifetimeManager)
        {
            _serviceLifetimeManager = serviceLifetimeManager;
        }

        public async ValueTask<Assistant> CreateOrRetrieveAssistantAsync()
        {
            var assistant = await AssistantService.RetrieveAsync(ConfigManager.AssistantConfig.AssistantId) ?? await AssistantService.CreateAsync(new Assistant()
                {
                    Name = "VSGPT",
                    Description = "c68c145d-b93a-4af3-8a98-f689efab069e (don't modify)",
                    Model = "gpt-4-1106-preview",
                    Instructions =
                        "You are a coding bot integrated into Visual Studio.\n" +
                        "File Modification:\n" +
                            "Request: I specify which file I want to modify and provide the changes needed.\n" +
                            "Response: You will provide the updated content for my review. You only respond with the updated code in the file.\n" +
                        "Review and Confirmation:\n" +
                            "Request: I review the provided content and confirm if it meets my requirements or request further modifications.\n" +
                            "Response: You will wait for your confirmation or additional instructions.\n" +
                        "Next Steps:\n" +
                            "Request: I Specify the next task or modifications I want to implement in the project.\n" +
                            "Response: You will provide the updated content and wait for my feedback. The updated content is also only the code in the file." +
                        "Request format " +
                        "1st line is a statement of filename.\n" +
                        "2nd line contains the requested modification to be applied to the file.\n" +
                        "From the 3rd line you get the current code which you need to memorize and modify. The code will be provided in triple quotes.\n" +
                        "The last line instructs you to output the modified content and only that, only code. This makes sure you don't forget the instructions.",
                    Metadata = new Dictionary<string, string>
                    {
                        { "purpose", "vsgpt" }, { "project", ConfigManager.AssistantConfig.ProjectId },
                    }
                });
            return assistant;
        }

        public async ValueTask<Thread> CreateOrRetrieveThreadAsync()
        {
            var thread = await ThreadService.RetrieveAsync(ConfigManager.AssistantConfig.ThreadId) ?? await ThreadService.CreateAsync(new Thread()
                {
                    Metadata = new Dictionary<string, string>
                    {
                        { "purpose", "vsgpt" }, { "project", ConfigManager.AssistantConfig.ProjectId },
                    }
                });
            return thread;
        }

        public async ValueTask<string> RequestAsync(string fileId, string request)
        {
            // Put message on the thread
            await MessageService.CreateAsync(new MessageRequest()
            {
                ThreadId = ConfigManager.AssistantConfig.ThreadId,
                Content = $"Open file {fileId} and make modifications following the instruction:\n{request}\noutput the entire updated content, only this file and only give me code",
                Role = "user",
            });

            // Run the thread
            var run = await RunService.CreateAsync(new Run()
            {
                ThreadId = ConfigManager.AssistantConfig.ThreadId,
                AssistantId = ConfigManager.AssistantConfig.AssistantId
            });
            var runId = run.Id;

            // Wait until run is ended
            while(run.Status == "queued")
            {
                System.Threading.Thread.Sleep(500);
                run = await RunService.RetrieveAsync(ConfigManager.AssistantConfig.ThreadId, runId);
            }

            // Return the response
            var response = (await MessageService.ListAsync(ConfigManager.AssistantConfig.ThreadId)).OrderByDescending(m => m.CreatedAt).First().Content[0].Text;
            return response.ToString();
        }
    }
}