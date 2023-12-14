using Lionence.VSGPT.Configurations;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Lionence.VSGPT.Services.Managers
{
    public sealed class ConfigManager
    {
        private readonly string _projectDirectory;

        public ConfigManager(string projectDirectory)
        {
            _projectDirectory = projectDirectory;
            Load();
        }

        public AssistantConfig AssistantConfig { get; set; }
        public ExtensionConfig ExtensionConfig { get; set; }

        public void Load()
        {
            GetDirectories(out string extensionDir, out var assistantConfigFile, out var extensionConfigFile);

            if (!Directory.Exists(extensionDir))
            {
                Directory.CreateDirectory(extensionDir);
            }

            if(!File.Exists(assistantConfigFile))
            {
                AssistantConfig = new AssistantConfig()
                {
                    ProjectId = new Guid().ToString(),
                };
                File.WriteAllText(assistantConfigFile, JsonConvert.SerializeObject(AssistantConfig));
            }
            else
            {
                AssistantConfig = JsonConvert.DeserializeObject<AssistantConfig>(assistantConfigFile);
            }

            if (!File.Exists(extensionConfigFile))
            {
                ExtensionConfig = new ExtensionConfig()
                {
                    ApiKey = "",
                };
                File.WriteAllText(extensionConfigFile, JsonConvert.SerializeObject(ExtensionConfig));
            }
            else
            {
                ExtensionConfig = JsonConvert.DeserializeObject<ExtensionConfig>(extensionConfigFile);
            }
        }

        public void Save()
        {
            GetDirectories(out _, out var assistantConfigFile, out var extensionConfigFile);
            File.WriteAllText(assistantConfigFile, JsonConvert.SerializeObject(AssistantConfig));
            File.WriteAllText(extensionConfigFile, JsonConvert.SerializeObject(ExtensionConfig));
        }

        private void GetDirectories(out string extensionDir, out string assistantConfigFile, out string extensionConfigFile)
        {
            var vsDir = $"{_projectDirectory}\\.vs";
            extensionDir = $"{vsDir}\\vsassistant_c68c145d-b93a-4af3-8a98-f689efab069e";
            assistantConfigFile = $"{extensionDir}\\assistant.json";
            extensionConfigFile = $"{extensionDir}\\extension.json";
        }
    }
}
