using EnvDTE;
using EnvDTE80;
using Lionence.VSGPT.Commands;
using Lionence.VSGPT.Windows;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.IO;
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
        private ChatGPTCommand _command;
        private DTE2 _dte;

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            _command = await ChatGPTCommand.InitializeAsync(this);

            _dte = await this.GetServiceAsync<DTE, DTE2>();

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(DisposalToken);
            _windowEvents = _dte.Events.WindowEvents;
            _windowEvents.WindowActivated += HandleWindowActivated;
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
