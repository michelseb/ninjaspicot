using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Extensions;
using ZepLink.RiceNinja.ServiceLocator.Services;
using ZepLink.RiceNinja.ServiceLocator.Services.Impl;

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

        private IDictionary<Type, IPoolService> _poolServices = new Dictionary<Type, IPoolService>();

        private static ServiceFinder _instance;
        public static ServiceFinder Instance { get { if (_instance == null) _instance = new ServiceFinder(); return _instance; } }

        public T Get<T>() where T : IGameService
        {
            var key = typeof(T).FullName;

            if (!_services.ContainsKey(key))
            {
                Debug.LogError($"Service {key} was not registered and cannot be used");
                return default(T);
            }

            return (T)_services[key];
        }

        public T PoolFor<T>(Vector3 position, Quaternion rotation, float size, string modelName = default) where T : IPoolable
        {
            var key = typeof(T);

            if (_poolServices.ContainsKey(key))
                return (T)_poolServices[key].Pool(position, rotation, size, modelName);

            var service = _poolServices.FirstOrDefault(x => x.Key.IsAssignableFrom(typeof(T))).Value ?? Register(MakePoolService<T>());

            return (T)service.Pool(position, rotation, size, modelName);
        }

        private IPoolService MakePoolService<T>() where T : IPoolable
        {
            return new PoolService<T>();
        }

        public bool IsRegistered<T>()
        {
            return _services.ContainsKey(typeof(T).FullName);
        }

        public T Register<T>(T service) where T: IGameService
        {
            string key = typeof(T).FullName;
            
            if (_services.ContainsKey(key))
            {
                Debug.LogError($"Attempted to register service of type {key} which is already registered");
                return service;
            }

            _services.Add(key, service);

            if (service.GetType().IsInstanceOfGenericType(typeof(PoolService<>)))
            {
                _poolServices.Add(typeof(T).GetGenericArgument(), (IPoolService)service);
            }

            service.Init(_serviceParent);

            Debug.Log($"Service {key} registered");

            return service;
        }

        public void Unregister<T>() where T : IGameService
        {
            var key = typeof(T).FullName;
            
            if (!_services.ContainsKey(key))
            {
                Debug.LogError($"Attempted to unregister service of type {key} which is not registered");
                return;
            }

            _services.Remove(key);
        }
    }
}