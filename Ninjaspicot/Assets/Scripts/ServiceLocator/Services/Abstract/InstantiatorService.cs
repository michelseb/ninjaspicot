using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics;
using ZepLink.RiceNinja.Manageables;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Abstract
{
    public abstract class InstantiatorService<T, U> : CollectionService<T, U>, IInstantiatorService<T, U> where U : class, IManageable<T>
    {
        protected abstract string ModelPath { get; }

        protected Dictionary<T, U> _models = new Dictionary<T, U>();
        protected Transform _parent;
        protected Transform _instancesParent;

        public override void Init(Transform parent)
        {
            base.Init(parent);

            var parentName = string.Empty;

            if (string.IsNullOrEmpty(ModelPath))
            {
                Debug.LogError($"ModelPath wasn't set for service {GetType()}");
                return;
            }

            var models = Resources.LoadAll(ModelPath).Cast<U>().ToArray();
            _models = models.ToDictionary(t => t.Id, t => t);

            parentName = ModelPath.Split('/').Last();
            _parent = GameObject.Find(parentName)?.transform ?? new GameObject(parentName).transform;

            if (_parent == null)
                Debug.LogError($"No transform name matching instantiator parent for service {GetType()}");

            _instancesParent = GameObject.Find("Instances")?.transform ?? new GameObject("Instances").transform;
            _parent.SetParent(_instancesParent);
        }

        public virtual U Create(U model, Vector3 position, Quaternion rotation, Transform parent = default)
        {
            if (EqualityComparer<U>.Default.Equals(model, default(U)) || model is not Dynamic dynamic)
                return default(U);

            var instance = UnityEngine.Object.Instantiate(dynamic, position, rotation, parent ?? _parent);

            var component = instance.GetComponent<U>();
            Add(component);

            return component;
        }

        public U Create(U model, Vector3 position, Transform parent = default)
        {
            return Create(model, position, Quaternion.identity, parent);
        }

        public U Create(U model, Transform _parent = default)
        {
            return Create(model, Vector3.zero, _parent);
        }

        public U Equip(Type componentType, Transform instance)
        {
            if (componentType is not U)
            {
                Debug.LogError($"Service {GetType()} could not add component of type {componentType.Name}");
                return default(U);
            }

            if (instance.gameObject.TryGetComponent(componentType, out Component component))
            {
                Debug.LogError($"Service {GetType()} trying to add already existing component {componentType.Name} to instance {instance.name}");
                return component as U;
            }

            var result = instance.gameObject.AddComponent(componentType) as U;
            Add(result);

            return result;
        }

        public U GetModelByName(string name)
        {
            return _models.Values.FirstOrDefault(x => x.Name == name);
        }

        public U GetRandomInstance()
        {
            var id = InstancesDictionary.Keys.OrderBy(x => Guid.NewGuid()).FirstOrDefault();

            return FindById(id);
        }

        public U GetRandomModel()
        {
            return _models.Values.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
        }
    }
}
