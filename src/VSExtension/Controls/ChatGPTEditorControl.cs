using System.Windows;
using System.Windows.Controls;

namespace Lionence.VSGPT.Control
{
    public sealed partial class ChatGPTEditorControl : UserControl
    {
        private string FileContent { get; set; } = string.Empty;
        private string FileName { get; set; } = string.Empty;
        private string RelativePath { get; set; } = string.Empty;

        public ChatGPTEditorControl()
        {
            InitializeComponent();
        }

        public void ReadFile(string filePath, string relativePath, string fileName)
        {
            FileNameLabel.Content = RelativePath = relativePath;
            FileContent = System.IO.File.ReadAllText(filePath);
            FileName = fileName;
        }

        public void ClearFile()
        {
            FileNameLabel.Content = string.Empty;
            FileContent = string.Empty;
            FileName = string.Empty;
        }

        private void GenerateNewFileButton_Click(object sender, RoutedEventArgs e)
        {
            if(!CheckInput())
                return;

        }

        private void GenerateDiffButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckInput())
                return;

        }
        
        private bool CheckInput()
        {
            if (ApiKeyPasswordBox.SecurePassword.Length == 0)
            {
                MessageBox.Show("ApiKey should be set!");
                return false;
            }
            if (PromptTextBox.Text.Length == 0)
            {
                MessageBox.Show("Prompt should be set!");
                return false;
            }
            if (FileNameLabel.ContentStringFormat.Length == 0)
            {
                MessageBox.Show("A file should be active!");
                return false;
            }
            return true;
        }
    }
}