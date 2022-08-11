using System.Linq;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class PoolService<T> : InstanceService<T>, IPoolService<T> where T : IPoolable
    {
        public override string Name => $"PoolService_{typeof(T).Name}";
        protected override string ModelPath => $"Poolables/{typeof(T).Name}";

        public T PoolByName(string name, Vector3 position, Quaternion rotation, float size)
        {
            var model = _models.Values.FirstOrDefault(x => x.Name == name);

            if (model == null)
                return default;

            return PoolAt(position, rotation, size, model);
        }

        public T PoolByName(string name, Vector3 position, Quaternion rotation)
        {
            return PoolByName(name, position, rotation, 1);
        }

        public T PoolByName(string name, Vector3 position)
        {
            return PoolByName(name, position, Quaternion.identity);
        }

        public T PoolByName(string name)
        {
            return PoolByName(name, Vector3.zero);
        }

        public IPoolable Pool(Vector3 position, Quaternion rotation, float size, IPoolable model)
        {
            return PoolAt(position, rotation, size, (T)model);
        }

        public IPoolable Pool(Vector3 position, Quaternion rotation, IPoolable model)
        {
            return PoolAt(position, rotation, (T)model);
        }

        public IPoolable Pool(Vector3 position, IPoolable model)
        {
            return PoolAt(position, (T)model);
        }

        public T PoolAt(Vector3 position, Quaternion rotation, float size, T model = default)
        {
            var poolable = model != null ? Create(model) : GetDefaultPoolable();

            if (poolable == null)
                return default;

            poolable.Wake();
            poolable.Pool(position, rotation, size);

            return poolable;
        }

        public T PoolAt(Vector3 position, Quaternion rotation, T model = default)
        {
            return PoolAt(position, rotation, 1, model);
        }

        public T PoolAt(Vector3 position, T model = default)
        {
            return PoolAt(position, Quaternion.identity, 1, model);
        }

        private T GetDefaultPoolable()
        {
            var poolable = Collection.FirstOrDefault(p => p is MonoBehaviour mono && !mono.gameObject.activeSelf);

            if (poolable != null)
                return poolable;

            // Create poolable if doesn't exist
            var poolableModel = _models.Values.FirstOrDefault(x => x is T);

            if (poolableModel == null)
                return default;

            return Create(poolableModel);
        }
    }
}
