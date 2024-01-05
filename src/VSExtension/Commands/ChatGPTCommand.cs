using Lionence.VSGPT.Services.Managers;
using Lionence.VSGPT.Windows;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;

namespace Lionence.VSGPT.Commands
{
    internal sealed class ChatGPTCommand : IChatGPTCommand
    {
        private static Guid CommandSet => new Guid("35477f6e-e7c2-4b67-a2f7-0f1a2bae42ec");

        private readonly AsyncPackage _package;

        private readonly static int CommandId = 4129;

        public ChatGPTEditorWindow Window;

        public ChatGPTCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            _package = package;

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(ExecuteAsync, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        private async void ExecuteAsync(object sender, EventArgs e)
        {
            if (Window == null)
            {
                Window = (await _package.ShowToolWindowAsync(
                        toolWindowType: typeof(ChatGPTEditorWindow),
                        id: 0,
                        create: true,
                        cancellationToken: _package.DisposalToken)) as ChatGPTEditorWindow;
               
                if (Window is null || Window.Frame is null)
                {
                    throw new NotSupportedException("Cannot create tool window");
                }
            }
        }
    }
}
