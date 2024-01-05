using Lionence.VSGPT.Control;
using Lionence.VSGPT.Services.Managers;
using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;

namespace Lionence.VSGPT.Windows
{
    [Guid("bbc33f4a-54d9-44b6-a413-36efdbb66421")]
    public sealed class ChatGPTEditorWindow : ToolWindowPane, IChatGPTEditorWindow
    {
        private readonly ServiceLifetimeManager _serviceLifetimeManager;

        public WorkflowManager WorkflowManager => _serviceLifetimeManager.Get<WorkflowManager>();
        public FileManager FileManager => _serviceLifetimeManager.Get<FileManager>();
        public ConfigManager ConfigManager => _serviceLifetimeManager.Get<ConfigManager>();

        public ChatGPTEditorWindow() : base(null)
        {
            Caption = "ChatGPT";
            Content = new ChatGPTEditorControl();
        }

        public ChatGPTEditorControl Control => Content as ChatGPTEditorControl;
    }
}
