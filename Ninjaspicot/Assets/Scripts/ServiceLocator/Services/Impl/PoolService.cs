using System;
using System.Linq;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class PoolService<T> : InstanceService<T>, IPoolService<T> where T : IPoolable
    {
        public override string Name => $"PoolService_{typeof(T).Name}";
        protected override string ModelPath => $"Poolables/{GetType().Name}";

        public T PoolAt(Vector3 position, Quaternion rotation, float size)
        {
            var poolable = GetPoolable();

            if (poolable == null)
                return default;

            poolable.Wake();
            poolable.Pool(position, rotation, size);

            return poolable;
        }

        public T PoolAt(Vector3 position, Quaternion rotation)
        {
            return PoolAt(position, rotation, 1);
        }

        public T PoolAt(Vector3 position)
        {
            return PoolAt(position, Quaternion.identity, 1);
        }

        private T GetPoolable()
        {
            var poolable = Collection.FirstOrDefault(p => p is MonoBehaviour mono && !mono.gameObject.activeSelf);

            if (poolable != null)
                return poolable;

            // Create poolable if doesn't exist
            var poolableModel = _models.Values.FirstOrDefault(x => x is T);

            if (poolableModel == null)
                return default;

            var service = ServiceFinder.Instance.GetCollectionFor<T>();

            return (T)service.Create(poolableModel);
        }
    }
}
