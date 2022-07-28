using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics;
using ZepLink.RiceNinja.Manageables;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public abstract class InstantiatorService<T> : CollectionService<T>, IInstantiatorService<T> where T : class, IManageable
    {
        protected abstract string ModelPath { get; }

        protected Dictionary<int, T> _models = new Dictionary<int, T>();
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

            var models = Resources.LoadAll(ModelPath).Cast<GameObject>().ToArray();
            _models = models.Select((model, index) => new { Index = index, Value = model.GetComponent<T>() }).ToDictionary(t => t.Index, t => t.Value);

            parentName = ModelPath.Split('/').Last();
            _parent = GameObject.Find(parentName)?.transform ?? new GameObject(parentName).transform;

            if (_parent == null)
                Debug.LogError($"No transform name matching instantiator parent for service {GetType()}");

            _instancesParent = GameObject.Find("Instances")?.transform ?? new GameObject("Instances").transform;
            _parent.SetParent(_instancesParent);
        }

        public virtual T Create(T model, Vector3 position, Quaternion rotation, Transform parent = default)
        {
            if (EqualityComparer<T>.Default.Equals(model, default(T)) || model is not Dynamic dynamic)
                return default(T);

            var instance = UnityEngine.Object.Instantiate(dynamic, position, rotation, parent ?? _parent);

            var component = instance.GetComponent<T>();
            Add(component);

            return component;
        }

        public T Create(T model, Vector3 position, Transform parent = default)
        {
            return Create(model, position, Quaternion.identity, parent);
        }

        public T Create(T model, Transform _parent = default)
        {
            return Create(model, Vector3.zero, _parent);
        }

        public T Equip(Type componentType, Transform instance)
        {
            if (componentType is not T)
            {
                Debug.LogError($"Service {GetType()} could not add component of type {componentType.Name}");
                return default(T);
            }

            if (instance.gameObject.TryGetComponent(componentType, out Component component))
            {
                Debug.LogError($"Service {GetType()} trying to add already existing component {componentType.Name} to instance {instance.name}");
                return component as T;
            }

            var result = instance.gameObject.AddComponent(componentType) as T;
            Add(result);

            return result;
        }

        public T GetModelByName(string name)
        {
            return _models.Values.FirstOrDefault(x => x.Name == name);
        }

        public T GetRandomInstance()
        {
            var id = InstancesDictionary.Keys.OrderBy(x => Guid.NewGuid()).FirstOrDefault();

            return FindById(id);
        }

        public T GetRandomModel()
        {
            var index = UnityEngine.Random.Range(0, _models.Count);

            return _models[index];
        }
    }
}
