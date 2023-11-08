using EnvDTE;
using EnvDTE80;
using Lionence.VSGPT.Commands;
using Lionence.VSGPT.Window;
using Microsoft.VisualStudio.Shell;
using System;
using System.IO.Packaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Lionence.VSGPT
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(ChatGPTEditorWindow))]
    public sealed class ChatGPTEditorPackage : AsyncPackage
    {
        public const string PackageGuidString = "a2373120-2267-47f1-8236-3154f8e6aa88";

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await ChatGPTCommand.InitializeAsync(this);

            var dte = await this.GetServiceAsync<DTE, DTE2>();
            var vsEvents = dte.Events;

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(DisposalToken);
            vsEvents.DocumentEvents.DocumentOpened += HandleDocumentOpened;
        }

        /// Runs when a document is opened
        private static void HandleDocumentOpened(Document document)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            MessageBox.Show(document.FullName);
        }
    }
}
