using Lionence.VSGPT.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lionence.VSGPT.Services
{
    internal sealed class GptManagerService
    {
        private readonly GptAssistantService _assistantService;
        private readonly GptFileService _fileService;
        private readonly GptThreadService _threadService;
        private readonly GptRunService _runService;
        private readonly GptMessageService _messageService;

        public GptManagerService(
            GptAssistantService assistantService,
            GptFileService fileService,
            GptThreadService threadService,
            GptRunService runService,
            GptMessageService messageService)
        {
            _assistantService = assistantService;
            _fileService = fileService;
            _threadService = threadService;
            _runService = runService;
            _messageService = messageService;
        }

        public async ValueTask<Assistant> CreateOrRetrieveAssistantAsync(string projectGuid)
        {
            var assistant = await _assistantService.RetrieveAsync(projectGuid);
            if (assistant == null)
            {
                assistant = await _assistantService.CreateAsync(new Assistant()
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
                        { "purpose", "vsgpt" }, { "project", projectGuid },
                    }
                });
            }
            return assistant;
        }

        public async ValueTask<Thread> CreateOrRetrieveThreadAsync(string projectGuid)
        {
            var thread = await _threadService.RetrieveAsync(projectGuid);
            if (thread == null)
            {
                thread = await _threadService.CreateAsync(new Thread()
                {
                    Metadata = new Dictionary<string, string>
                    {
                        { "purpose", "vsgpt" }, { "project", projectGuid },
                    }
                });
            }

            return thread;
        }

        public async ValueTask<File> CreateOrUpdateFileAsync(string filePath)
        {
            filePath = filePath.Replace("\\", "_").Replace("/", "_");
            var file = await _fileService.RetrieveAsync(filePath);
            if(file == null)
            {
                file = await _fileService.CreateAsync(new File()
                {
                    Filename = filePath,
                    Purpose = "assistant"
                });
            }
            return file;
        }

        public async ValueTask<string> RequestAsync(string threadId, string assistantId, string fileId, string request)
        {
            await _messageService.CreateAsync(new Message()
            {
                ThreadId = threadId,
                AssistantId = assistantId,
                Role = "user",
            });

            var run = await _runService.CreateAsync(new Run()
            {
                ThreadId = threadId,
                AssistantId = assistantId
            });
            var runId = run.Id;

            // Wait until run is ended
            while(run.Status == "queued")
            {
                System.Threading.Thread.Sleep(500);
                run = await _runService.RetrieveAsync(threadId, runId);
            }

            var response = (await _messageService.ListAsync(threadId)).OrderByDescending(m => m.CreatedAt).First();

            return response.ToString();
        }
    }
}