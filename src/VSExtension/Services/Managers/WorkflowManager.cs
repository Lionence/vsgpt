using Lionence.VSGPT.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lionence.VSGPT.Services.Managers
{
    public sealed class WorkflowManager
    {
        private readonly ConfigManager _configManager;
        private readonly GptAssistantService _assistantService;
        private readonly GptThreadService _threadService;
        private readonly GptRunService _runService;
        private readonly GptMessageService _messageService;

        public WorkflowManager(
            ConfigManager configManager,
            GptAssistantService assistantService,
            GptThreadService threadService,
            GptRunService runService,
            GptMessageService messageService)
        {
            _configManager = configManager;
            _assistantService = assistantService;
            _threadService = threadService;
            _runService = runService;
            _messageService = messageService;
        }

        public async ValueTask<Assistant> CreateOrRetrieveAssistantAsync()
        {
            var assistant = await _assistantService.RetrieveAsync(_configManager.AssistantConfig.AssistantId) ?? await _assistantService.CreateAsync(new Assistant()
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
                        { "purpose", "vsgpt" }, { "project", _configManager.AssistantConfig.ProjectId },
                    }
                });
            return assistant;
        }

        public async ValueTask<Thread> CreateOrRetrieveThreadAsync()
        {
            var thread = await _threadService.RetrieveAsync(_configManager.AssistantConfig.ThreadId) ?? await _threadService.CreateAsync(new Thread()
                {
                    Metadata = new Dictionary<string, string>
                    {
                        { "purpose", "vsgpt" }, { "project", _configManager.AssistantConfig.ProjectId },
                    }
                });
            return thread;
        }

        public async ValueTask<string> RequestAsync(string fileId, string request)
        {
            // Put message on the thread
            await _messageService.CreateAsync(new MessageRequest()
            {
                ThreadId = _configManager.AssistantConfig.ThreadId,
                Content = $"Open file {fileId} and make modifications following the instruction:\n{request}\noutput the entire updated content, only this file and only give me code",
                Role = "user",
            });

            // Run the thread
            var run = await _runService.CreateAsync(new Run()
            {
                ThreadId = _configManager.AssistantConfig.ThreadId,
                AssistantId = _configManager.AssistantConfig.AssistantId
            });
            var runId = run.Id;

            // Wait until run is ended
            while(run.Status == "queued")
            {
                System.Threading.Thread.Sleep(500);
                run = await _runService.RetrieveAsync(_configManager.AssistantConfig.ThreadId, runId);
            }

            // Return the response
            var response = (await _messageService.ListAsync(_configManager.AssistantConfig.ThreadId)).OrderByDescending(m => m.CreatedAt).First().Content[0].Text;
            return response.ToString();
        }
    }
}