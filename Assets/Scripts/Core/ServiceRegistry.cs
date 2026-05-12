using System;
using System.Collections.Generic;

namespace Frostline.Core
{
    public interface IServiceRegistry
    {
        public T GetService<T>() where T : class;
        public void RegisterService<T>(T service) where T : class;
    }
    public interface IRequireServices
    {
        public void Initialize(IServiceRegistry serviceRegistry);
    }

    public class ServiceRegistry : IServiceRegistry
    {
        private readonly Dictionary<Type, object> _services;

        public ServiceRegistry()
        {
            _services = new();
        }

        public T GetService<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out object service))
            {
                return service as T;
            }

            throw new InvalidOperationException($"Service of type {typeof(T).Name} is not registered");
        }

        public void RegisterService<T>(T service) where T : class
        {
            _services.Add(typeof(T), service);
        }
    }
}