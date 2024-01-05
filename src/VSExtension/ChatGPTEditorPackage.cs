using EnvDTE;
using EnvDTE80;
using Lionence.VSGPT.Commands;
using Lionence.VSGPT.Services;
using Lionence.VSGPT.Services.Managers;
using Lionence.VSGPT.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.IO;
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

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // Register base components
            _dte = await this.GetServiceAsync<DTE, DTE2>();
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(DisposalToken);
            _serviceLifetimeManager = new ServiceLifetimeManager(this);

            // Register non-changing components
            IServiceCollection services = new ServiceCollection();
            var httpClientFactoy = services.AddHttpClient().BuildServiceProvider().GetService<IHttpClientFactory>();
            _serviceLifetimeManager.Register(httpClientFactoy);

            // Register solution specific components
            if (!string.IsNullOrEmpty(_dte.Solution?.FullName))
            {
                _serviceLifetimeManager.Register(new ConfigManager(Path.GetDirectoryName(_dte.Solution.FullName)));
            }
        
            // Register dynamic components
            await _serviceLifetimeManager.RegisterAsync<GptAssistantService>();
            await _serviceLifetimeManager.RegisterAsync<GptThreadService>();
            await _serviceLifetimeManager.RegisterAsync<GptRunService>();
            await _serviceLifetimeManager.RegisterAsync<GptMessageService>();
            await _serviceLifetimeManager.RegisterAsync<GptFileService>();
            await _serviceLifetimeManager.RegisterAsync<WorkflowManager>();
            
            // Solution events
            _solutionEvents = _dte.Events.SolutionEvents;
            _solutionEvents.Opened += CreateSolutionSpecificServices;
            _solutionEvents.AfterClosing += RemoveSolutionSpecificServices;
            
            // Window events
            _windowEvents = _dte.Events.WindowEvents;
            _windowEvents.WindowActivated += HandleWindowActivated;

            // Register tool window
            if(!_serviceLifetimeManager.TryGet<ChatGPTEditorWindow>(out _))
            {
                _serviceLifetimeManager.Register(Activator.CreateInstance<ChatGPTEditorWindow>());
            }

            // Register command
            _command = new ChatGPTCommand(this, GetService(typeof(IMenuCommandService)) as OleMenuCommandService);
        }

        private async void CreateSolutionSpecificServices()
        {
            _serviceLifetimeManager.Register(new ConfigManager(Path.GetDirectoryName(_dte.Solution.FullName)));
            await _serviceLifetimeManager.RegisterAsync<FileManager>();
        }

        private void RemoveSolutionSpecificServices()
        {
            _serviceLifetimeManager.Remove<ConfigManager>();
            _serviceLifetimeManager.Remove<FileManager>();
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
