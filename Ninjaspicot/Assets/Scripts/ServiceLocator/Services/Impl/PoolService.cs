using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Abstract;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.ServiceLocator.Services.Abstract;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class PoolService<T> : CollectionService<int, T>, IPoolService<T> where T : IPoolable
    {
        public override string Name => $"PoolService_{typeof(T).Name}";
        protected virtual string ModelPath => $"Poolables/{typeof(T).Name}";

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
            if (model == null)
                return default(T);

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

        public IPoolable Pool(Vector3 position, Quaternion rotation, float size, string modelName = default)
        {
            return PoolAt(position, rotation, size, modelName);
        }

        public IPoolable Pool(Vector3 position, Quaternion rotation, string modelName = default)
        {
            return PoolAt(position, rotation, modelName);
        }

        public IPoolable Pool(Vector3 position, string modelName = default)
        {
            return PoolAt(position, modelName);
        }

        public virtual T PoolAt(Vector3 position, Quaternion rotation, float size, string modelName = default)
        {
            var poolable = GetDefaultPoolable(modelName);

            if (poolable == null)
                return default;

            poolable.Wake();
            poolable.Pool(position, rotation, size);

            return poolable;
        }

        public T PoolAt(Vector3 position, Quaternion rotation, string modelName = default)
        {
            return PoolAt(position, rotation, 1, modelName);
        }

        public T PoolAt(Vector3 position, string modelName = default)
        {
            return PoolAt(position, Quaternion.identity, 1, modelName);
        }

        protected T GetDefaultPoolable(string modelName)
        {
            var poolable = Collection
                .Where(x => IsModelValid(x.Name, modelName))
                .FirstOrDefault(p => p is MonoBehaviour mono && !mono.gameObject.activeSelf);

            if (poolable != null)
                return poolable;

            // Create poolable if doesn't exist
            var poolableModel = _models.Values.FirstOrDefault(x => IsModelValid(x.Name, modelName));

            if (poolableModel == null)
                return default;

            return Create(poolableModel);
        }

        protected bool IsModelValid(string name, string modelName)
        {
            name = name.Replace("(Clone)", string.Empty);
            return string.IsNullOrEmpty(modelName) || name == modelName;
        }
    }
}
