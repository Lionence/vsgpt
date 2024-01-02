using EnvDTE;
using EnvDTE80;
using Lionence.VSGPT.Commands;
using Lionence.VSGPT.Models;
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

        // Lifetime management
        private ServiceLifetimeManager _serviceLifetimeManager;
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

            _serviceLifetimeManager = new ServiceLifetimeManager(this);

            _assistantService = _serviceLifetimeManager.Register<GptAssistantService>(_configManager,httpClientFactoy);           
            _messageService = _serviceLifetimeManager.Register<GptMessageService>(_configManager, httpClientFactoy);
            _runService = _serviceLifetimeManager.Register<GptRunService>(_configManager, httpClientFactoy);
            _threadService = _serviceLifetimeManager.Register<GptThreadService>(_configManager, httpClientFactoy);
            _workflowManager = _serviceLifetimeManager.Register<WorkflowManager>(_configManager, _assistantService, _threadService, _runService, _messageService);
            
            _solutionEvents = _dte.Events.SolutionEvents;
            _solutionEvents.Opened += CreateSolutionSpecificServicesAsync;
            _solutionEvents.AfterClosing += RemoveSolutionSpecificServices;
            
            if(!string.IsNullOrEmpty(_dte.Solution?.FullName))
            {
                CreateSolutionSpecificServicesAsync();
            }

            _windowEvents = _dte.Events.WindowEvents;
            _windowEvents.WindowActivated += HandleWindowActivated;

            var window = _serviceLifetimeManager.Register<ChatGPTEditorWindow>();
            _command = new ChatGPTCommand(this,
                GetService(typeof(IMenuCommandService)) as OleMenuCommandService);
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
