using Lionence.VSGPT.Windows;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;

namespace Lionence.VSGPT.Commands
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ChatGPTCommand : IChatGPTCommand
    {
        private static Guid _CommandSet => new Guid("35477f6e-e7c2-4b67-a2f7-0f1a2bae42ec");

        private readonly AsyncPackage _package;

        private static int CommandId = 4129;

        public ChatGPTEditorWindow Window;

        public static async ValueTask<ChatGPTCommand> InitializeAsync(AsyncPackage package)
        {
            OleMenuCommandService commandService = await package.GetServiceAsync<IMenuCommandService, OleMenuCommandService>();
            return new ChatGPTCommand(package, commandService);
        }

        private ChatGPTCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            _package = package;

            var menuCommandID = new CommandID(_CommandSet, CommandId);
            var menuItem = new MenuCommand(Execute, menuCommandID);
            commandService.AddCommand(menuItem);

            Execute(null, null);
        }

        private void Execute(object sender, EventArgs e)
        {
            if (Window == null)
            {
                _package.JoinableTaskFactory.RunAsync(async delegate
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

                    _package.AddService(typeof(ChatGPTEditorWindow), (container, ct, type) => Task.FromResult(Window as object));
                }).Task.Wait();
            }
        }
    }
}
