using System.Linq;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class PoolService<T> : InstanceService<T>, IPoolService<T> where T : IPoolable
    {
        public override string Name => $"PoolService_{typeof(T).Name}";
        protected override string ModelPath => $"Poolables/{typeof(T).Name}";

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

        public T PoolAt(Vector3 position, Quaternion rotation, float size, string modelName = default)
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

        private T GetDefaultPoolable(string modelName)
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

        private bool IsModelValid(string name, string modelName)
        {
            name = name.Replace("(Clone)", string.Empty);
            return string.IsNullOrEmpty(modelName) || name == modelName;
        }
    }
}
