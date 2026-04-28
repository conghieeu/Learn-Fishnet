using System.Collections.Generic;

namespace MellowAbelson.Core.Services
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<System.Type, IService> _services = [];

        public static void Register<T>(T service) where T : IService
        {
            Type type = typeof(T);
            if (_services.ContainsKey(type))
            {
                throw new InvalidOperationException($"Service {type.Name} is already registered.");
            }
            _services[type] = service;
            service.Initialize();
        }

        public static T Get<T>() where T : IService
        {
            if (_services.TryGetValue(typeof(T), out IService service))
            {
                return (T)service;
            }
            throw new InvalidOperationException($"Service {typeof(T).Name} is not registered.");
        }

        public static void Unregister<T>() where T : IService
        {
            if (_services.TryGetValue(typeof(T), out IService service))
            {
                service.Shutdown();
                _services.Remove(typeof(T));
            }
        }

        public static void Clear()
        {
            foreach (IService service in _services.Values)
            {
                service.Shutdown();
            }
            _services.Clear();
        }
    }
}
