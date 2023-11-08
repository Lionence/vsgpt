using EnvDTE;
using EnvDTE80;
using Lionence.VSGPT.Window;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using System.Windows;

namespace Lionence.VSGPT.Commands
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ChatGPTCommand
    {
        public static readonly Guid CommandSet = new Guid("35477f6e-e7c2-4b67-a2f7-0f1a2bae42ec");
        private readonly AsyncPackage _package;

        public const int CommandId = 4129;

        public static ChatGPTCommand Instance { get; private set; }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            OleMenuCommandService commandService = await package.GetServiceAsync<IMenuCommandService, OleMenuCommandService>();
            Instance = new ChatGPTCommand(package, commandService);
        }

        private ChatGPTCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        private void Execute(object sender, EventArgs e)
        {
            _package.JoinableTaskFactory.RunAsync(async delegate
            {
                ChatGPTEditorWindow window = (await _package.ShowToolWindowAsync(typeof(ChatGPTEditorWindow), 0, true, _package.DisposalToken)) as ChatGPTEditorWindow;
                if ((null == window) || (null == window.Frame))
                {
                    throw new NotSupportedException("Cannot create tool window");
                }
            });
        }
    }
}
