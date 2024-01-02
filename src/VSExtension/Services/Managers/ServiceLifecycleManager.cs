using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Lionence.VSGPT.Services.Managers
{
    public class ServiceLifetimeManager
    {
        private readonly AsyncPackage _package;

        public ServiceLifetimeManager(AsyncPackage package)
        {
            _package = package;
        }

        public T Get<T>()
            where T : class
        {
            return _package.GetService<T>();
        }

        public T Register<T>(params object[] args)
            where T : class
        {
            var obj = Activator.CreateInstance(typeof(T), args);
            _package.AddService(typeof(T), (container, ct, type) => Task.FromResult(obj));
            return obj as T;
        }

        public void Remove<T>()
            where T : class
        {
            _package.RemoveService(typeof(T));
        }

        public T Reset<T>(params object[] args)
            where T : class
        {
            Remove<T>();
            return Register<T>(args);
        }

        public async void CreateSolutionSpecificServicesAsync(DTE2 dte)
        {
            Register<ConfigManager>(Path.GetDirectoryName(dte.Solution.FullName));
            Register<FileManager>(, _fileService, _assistantService);
        }

        public void RemoveSolutionSpecificServices()
        {
            RemoveService(typeof(ConfigManager));
            RemoveService(typeof(FileManager));
        }
    }
}
