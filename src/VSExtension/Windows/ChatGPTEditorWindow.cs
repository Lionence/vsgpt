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
        public WorkflowManager WorkflowManager { get; set; }
        public FileManager FileManager { get; set; }
        public ConfigManager ConfigManager { get; set; }

        public ChatGPTEditorWindow() : base(null)
        {
            Caption = "ChatGPT";
            WorkflowManager = GetService(typeof(WorkflowManager)) as WorkflowManager;
            FileManager = GetService(typeof(FileManager)) as FileManager; ;
            ConfigManager = GetService(typeof(ConfigManager)) as ConfigManager; ;

            Content = new ChatGPTEditorControl(WorkflowManager, FileManager, ConfigManager);
        }

        public ChatGPTEditorControl Control => Content as ChatGPTEditorControl;
    }
}
