using EnvDTE;
using EnvDTE80;
using Lionence.VSGPT.Commands;
using Lionence.VSGPT.Windows;
using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Lionence.VSGPT
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(ChatGPTEditorWindow))]
    public sealed class ChatGPTEditorPackage : AsyncPackage
    {
        public const string PackageGuidString = "a2373120-2267-47f1-8236-3154f8e6aa88";
        private WindowEvents _windowEvents;
        private ChatGPTCommand _command;

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            _command = await ChatGPTCommand.InitializeAsync(this);

            var dte = await this.GetServiceAsync<DTE, DTE2>();

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(DisposalToken);
            _windowEvents = dte.Events.WindowEvents;
            _windowEvents.WindowActivated += HandleWindowActivated;
        }

        private void HandleWindowActivated(Window windowInFocus, EnvDTE.Window windowLostFocus)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (windowInFocus.Document != null)
            {
                var control = this.GetService<ChatGPTEditorWindow, IChatGPTEditorWindow>().Control;
                control.ReadFile(windowInFocus.Document.FullName);
            }
        }
    }
}
