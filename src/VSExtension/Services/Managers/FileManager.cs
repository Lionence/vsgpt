using Lionence.VSGPT.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lionence.VSGPT.Services.Managers
{
    public class FileManager
    {
        private readonly ServiceLifetimeManager _serviceLifetimeManager;
        private ICollection<File> _localFiles = new List<File>();

        private ConfigManager ConfigManager => _serviceLifetimeManager.Get<ConfigManager>();
        private GptFileService FileService => _serviceLifetimeManager.Get<GptFileService>();
        private GptAssistantService AssistantService => _serviceLifetimeManager.Get<GptAssistantService>();

        public FileManager(ServiceLifetimeManager serviceLifetimeManager)
        {
            _serviceLifetimeManager = serviceLifetimeManager;
        }

        public async Task InitializeAsync(Assistant assistant)
        {
            _localFiles = (await FileService.ListAsync()).Where(serverFile => assistant.FileIds.Contains(serverFile.Id)).ToList();
        }

        public async Task SynchronizeFilesAsync(Assistant assistant)
        {
            var assistantFiles = (await FileService.ListAsync()).Where(serverFile => assistant.FileIds.Contains(serverFile.Id)).ToList();

            // Upload new files
            var newFiles = await Task.WhenAll(
                _localFiles.Where(localFile => !assistant.FileIds.Contains(localFile.Id))
                    .Select(f => FileService.CreateAsync(f))
                    .Select(vt => vt.AsTask()));

            // Update existing files
            var updatedFiles = _localFiles
                .Where(localFile => assistantFiles
                    .Any(serverFile => localFile.Filename == serverFile.Filename
                                    && localFile.CreatedAt > serverFile.CreatedAt));
            await Task.WhenAll(updatedFiles.Select(f => FileService.DeleteAsync(f.Id)).Select(vt => vt.AsTask()));
            updatedFiles = await Task.WhenAll(updatedFiles.Select(f => FileService.CreateAsync(f)).Select(vt => vt.AsTask()));

            // Unsync deleted files
            var deletedFiles = await Task.WhenAll(
                assistantFiles.Where(serverFile => !_localFiles.Any(localFile => localFile.Id == serverFile.Id))
                    .Select(file => FileService.DeleteAsync(file.Id))
                    .Select(vt => vt.AsTask()));

            // Update assistant
            assistantFiles.RemoveAll(af => deletedFiles.Any(df => df.Id == af.Id));
            assistantFiles = assistantFiles.Concat(newFiles).OrderBy(f => f.CreatedAt).ToList();
            if(assistantFiles.Count > 20)
            {
                assistantFiles.RemoveRange(0, assistantFiles.Count - 20);
            }
            assistant.FileIds = assistantFiles.Select(f => f.Id).ToList();
            await AssistantService.ModifyAsync(assistant);

            ConfigManager.AssistantConfig.AttachedFiles = assistant.FileIds;
        }

        public void FileOpened(string filePath)
        {
            if (!_localFiles.Any(f => f.Filename == filePath))
            {
                if (_localFiles.Count > 20)
                {
                    _localFiles.Remove(_localFiles.First());
                }
                _localFiles.Append(new File()
                {
                    Filename = filePath.Replace("\\", "_").Replace('/', '_'),
                    Purpose = "assistant"
                });
            }
        }

        public void FileCreated(string filePath)
            => FileOpened(filePath);

        public void FileDeleted(string filePath)
        {
            var file = _localFiles.SingleOrDefault(f => f.Filename == filePath);
            if (file != null)
            {
                _localFiles.Remove(file);
            }
        }
    }
}
