using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;
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

        private IDictionary<string, IGameService> _poolServices = new Dictionary<string, IGameService>();

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

        public IPoolService<T> GetPoolServiceFor<T>() where T : IPoolable
        {
            var key = typeof(T).FullName;

            if (_poolServices.ContainsKey(key))
                return (IPoolService<T>)_poolServices[key];

            var result = GetAssignableCollection<T>() ?? Register(new PoolService<T>());

            _poolServices.Add(key, result);

            return result;
        }

        public IPoolService<T> GetAssignableCollection<T>() where T : IPoolable
        {
            var generics = _services.Where(x => x.GetType().IsGenericType).Select(y => y.GetType().GetGenericArguments()[1]).ToArray();

            var result = _services.Values.FirstOrDefault(x =>
            {
                var type = x.GetType();

                return type.IsGenericType &&
                    type.GetGenericTypeDefinition() == typeof(T);// &&
                    //typeof(T).IsAssignableFrom(type.GetGenericArguments()[0]);

            });

            return (IPoolService<T>)result;
        }

        public bool IsRegistered<T>()
        {
            return _services.ContainsKey(typeof(T).FullName);
        }

        //public T RegisterCollection<T, U>(T service) where T : ICollectionService where U : IPoolable
        //{
        //    _poolServices.Add(typeof(U), service);
        //    return Register(service);
        //}


        public IPoolService<T> GetCollectionFor<T>() where T : IPoolable
        {
            var name = typeof(T).FullName;

            if (_poolServices.ContainsKey(name))
                return (IPoolService<T>)_poolServices[name];

            var key = _poolServices.Keys.FirstOrDefault();//x => x.IsAssignableFrom(typeof(T)));

            IPoolService<T> result;

            if (key != null && _poolServices.ContainsKey(key))
            {
                result = (IPoolService<T>)_poolServices[key];
                _poolServices.Add(key, result);
            }
            else
            {
                result = Register<IPoolService<T>>(new PoolService<T>());
            }

            return result;
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

            if (service.GetType() == typeof(PoolService<>))
            {
                _poolServices.Add(key, service);
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