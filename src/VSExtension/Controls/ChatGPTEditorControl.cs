using System.Windows;
using System.Windows.Controls;

namespace Lionence.VSGPT.Control
{
    public sealed partial class ChatGPTEditorControl : UserControl
    {
        private string FileContent { get; set; } = string.Empty;
        private string FileName { get; set; } = string.Empty;

        public ChatGPTEditorControl()
        {
            InitializeComponent();
        }

        public void ReadFile(string filePath)
        {
            FileNameLabel.Content = filePath;
            FileContent = System.IO.File.ReadAllText(filePath);
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}