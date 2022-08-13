using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Abstract;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.ServiceLocator.Services.Abstract;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class InstanceService<T> : CollectionService<int, T>, IInstanceService<T> where T : IDynamic
    {
        protected virtual string ModelPath { get; } = string.Empty;

        protected Dictionary<int, T> _models = new Dictionary<int, T>();

        private Transform _instancesParent;
        public virtual Transform InstancesParent 
        { 
            get 
            {
                if (BaseUtils.IsNull(_instancesParent))
                {
                    _instancesParent = GameObject.Find("Instances")?.transform ??
                                       new GameObject("Instances").transform;
                }
                return _instancesParent; 
            } 
        }

        public override void Init(Transform parent)
        {
            base.Init(parent);

            var parentName = string.Empty;

            if (string.IsNullOrEmpty(ModelPath))
            {
                Debug.LogError($"ModelPath wasn't set for service {GetType()}");
                return;
            }

            var resources = Resources.LoadAll(ModelPath);

            if (resources == null || resources.Length == 0)
                return;

            var models = resources.Cast<GameObject>().Select(x => x.GetComponent<T>());
            _models = models.ToDictionary(t => t.Id, t => t);
        }

        public T Create(T model, Vector3 position, Quaternion rotation)
        {
            if (EqualityComparer<T>.Default.Equals(model, default(T)) || model is not Dynamic dynamic)
                return default(T);

            var parent = model.Parent;
            parent.SetParent(InstancesParent);

            var instance = Object.Instantiate(dynamic, position, rotation, model.Parent);

            var component = instance.GetComponent<T>();
            Add(component);

            return component;
        }

        public T Create(T model, Vector3 position)
        {
            return Create(model, position, Quaternion.identity);
        }

        public T Create(T model)
        {
            return Create(model, Vector3.zero);
        }

        public T GetModelByName(string name)
        {
            return _models.Values.FirstOrDefault(x => x.Name == name);
        }
    }
}
