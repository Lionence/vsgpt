using EnvDTE;
using EnvDTE80;
using Lionence.VSGPT.Commands;
using Lionence.VSGPT.Services;
using Lionence.VSGPT.Services.Core;
using Lionence.VSGPT.Services.Managers;
using Lionence.VSGPT.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.IO;
using System.IO.Packaging;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Lionence.VSGPT
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(ChatGPTEditorWindow))]
    internal sealed class ChatGPTEditorPackage : AsyncPackage
    {
        public const string PackageGuidString = "a2373120-2267-47f1-8236-3154f8e6aa88";
        private WindowEvents _windowEvents;
        private SolutionEvents _solutionEvents;
        private DTE2 _dte;
        private ChatGPTCommand _command; // Bugfix: w/o this GC deletes the command and the extension won't work

        // Dependency injection members
        private GptAssistantService _assistantService { get; set; }
        private GptFileService _fileService { get; set; }
        private GptMessageService _messageService { get; set; }
        private GptRunService _runService { get; set; }
        private GptThreadService _threadService { get; set; }
        private ConfigManager _configManager { get; set; }
        private FileManager _fileManager { get; set; }
        private WorkflowManager _workflowManager { get; set; }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            _dte = await this.GetServiceAsync<DTE, DTE2>();

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(DisposalToken);

            IServiceCollection services = new ServiceCollection();
            var httpClientFactoy = services.AddHttpClient().BuildServiceProvider().GetService<IHttpClientFactory>();

            _assistantService = new GptAssistantService(_configManager, httpClientFactoy);
            AddService(typeof(GptAssistantService), (container, ct, type) => Task.FromResult(_assistantService as object));
            _messageService = new GptMessageService(_configManager, httpClientFactoy);
            AddService(typeof(GptMessageService), (container, ct, type) => Task.FromResult(_messageService as object));
            _runService = new GptRunService(_configManager, httpClientFactoy);
            AddService(typeof(GptRunService), (container, ct, type) => Task.FromResult(_runService as object));
            _threadService = new GptThreadService(_configManager, httpClientFactoy);
            AddService(typeof(GptThreadService), (container, ct, type) => Task.FromResult(_threadService as object));
            _workflowManager = new WorkflowManager(_configManager, _assistantService, _threadService, _runService, _messageService);
            AddService(typeof(WorkflowManager), (container, ct, type) => Task.FromResult(_workflowManager as object));

            _solutionEvents = _dte.Events.SolutionEvents;
            _solutionEvents.Opened += SolutionEvents_Opened;
            _solutionEvents.AfterClosing += SolutionEvents_AfterClosing;

            _windowEvents = _dte.Events.WindowEvents;
            _windowEvents.WindowActivated += HandleWindowActivated;

            var window = new ChatGPTEditorWindow(_workflowManager, _fileManager, _configManager);
            AddService(typeof(ChatGPTEditorWindow), (container, ct, type) => Task.FromResult(window as object));
            _command = await ChatGPTCommand.InitializeAsync(this);
        }

        private async void SolutionEvents_Opened()
        {
            // Create new ConfigManager
            _configManager = new ConfigManager(Path.GetDirectoryName(_dte.Solution.FullName));
            _configManager.Load();
            AddService(typeof(ConfigManager), (container, ct, type) => Task.FromResult(_configManager as object));
            // Create new FileManager
            _fileManager = new FileManager(_configManager, _fileService, _assistantService);
            await _fileManager.InitializeAsync(await _assistantService.RetrieveAsync(_configManager.AssistantConfig.AssistantId));
            AddService(typeof(FileManager), (container, ct, type) => Task.FromResult(_fileManager as object));
        }

        private void SolutionEvents_AfterClosing()
        {
            RemoveService(typeof(ConfigManager));
            RemoveService(typeof(FileManager));
        }

        private void HandleWindowActivated(Window windowInFocus, Window windowLostFocus)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if(windowInFocus.Caption == "Solution Explorer")
            {
                var control = this.GetService<ChatGPTEditorWindow, IChatGPTEditorWindow>().Control;
                control.ClearFile();
            }

            if (windowInFocus.Document != null)
            {
                string solutionDirectory = Path.GetDirectoryName(_dte.Solution.FullName);
                string relativeDocumentPath = windowInFocus.Document.FullName.Replace(solutionDirectory, string.Empty);
                var control = this.GetService<ChatGPTEditorWindow, IChatGPTEditorWindow>().Control;
                control.ReadFile(windowInFocus.Document.FullName, relativeDocumentPath, windowInFocus.Document.Name);
            }
        }
    }
}
