using System.Collections.Generic;
using UnityEngine;
using ZepLink.RiceNinja.ServiceLocator.Services;

namespace ZepLink.RiceNinja.ServiceLocator
{
    public class ServiceFinder
    {
        private ServiceFinder() 
        {
            _serviceParent = GameObject.Find("Services")?.transform ?? new GameObject("Services").transform;
        }

        private Transform _serviceParent;
        
        private IDictionary<string, IGameService> _services = new Dictionary<string, IGameService>();

        private static ServiceFinder _instance;
        public static ServiceFinder Instance { get { if (_instance == null) _instance = new ServiceFinder(); return _instance; } }

        public T Get<T>() where T : IGameService
        {
            var key = typeof(T).Name;

            if (!_services.ContainsKey(key))
            {
                Debug.LogError($"Service {key} was not registered and cannot be used");
                return default(T);
            }

            return (T)_services[key];
        }

        public T Register<T>(T service) where T: IGameService
        {
            string key = typeof(T).Name;
            
            if (_services.ContainsKey(key))
            {
                Debug.LogError($"Attempted to register service of type {key} which is already registered");
                return service;
            }

            _services.Add(key, service);

            service.Init(_serviceParent);

            Debug.Log($"Service {key} registered");

            return service;
        }

        public void Unregister<T>() where T : IGameService
        {
            var key = typeof(T).Name;
            
            if (!_services.ContainsKey(key))
            {
                Debug.LogError($"Attempted to unregister service of type {key} which is not registered");
                return;
            }

            _services.Remove(key);
        }
    }
}