using Microsoft.VisualStudio.Shell;
using System;
using System.Threading.Tasks;

namespace Lionence.VSGPT.Services.Managers
{
    public class ServiceLifetimeManager
    {
        public static Lazy<ServiceLifetimeManager> Instance { get; private set; }

        private readonly AsyncPackage _package;

        public ServiceLifetimeManager(AsyncPackage package)
        {
            _package = package;
            Instance = new Lazy<ServiceLifetimeManager>(() => this, true);
        }

        public bool TryGet<T>(out T instance)
            where T : class
        {
            try
            {
                instance = _package.GetService<T, T>();
                return true;
            }
            catch
            {
                instance = null;
                return false;
            }
        }

        public T Get<T>()
            where T : class
        {
            return _package.GetService<T, T>();
        }

        public void Register<T>(T instance)
            where T : class
        {
            _package.AddService(typeof(T), (container, ct, type) => Task.FromResult(instance as object));
        }

        public async Task RegisterAsync<T>()
            where T : class
        {
            var obj = Activator.CreateInstance(typeof(T), this);
            _package.AddService(typeof(T), (container, ct, type) => Task.FromResult(obj));
        }

        public void Remove<T>()
            where T : class
        {
            _package.RemoveService(typeof(T));
        }
    }
}
