using System.Linq;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class PoolService : InstantiatorService<IPoolable>, IPoolService
    {
        protected override string ModelPath => "Poolables";

        //Returns first deactivated poolable of chosen type. If none, instanciate one
        public T GetPoolable<T>(Vector3 position, Quaternion rotation, float size = 1f, Transform parent = null, bool defaultParent = true) where T : IPoolable
        {
            var poolable = Collection.FirstOrDefault(p =>
                p is T &&
                !((MonoBehaviour)p).gameObject.activeSelf);

            if (poolable != null)
                return Pool((T)poolable, position, rotation, size);

            // Create poolable if doesn't exist
            var poolableModel = _models.Values.FirstOrDefault();

            if (poolableModel == null)
                return default;

            poolable = Create(poolableModel, parent ?? _parent);

            return Pool((T)poolable, position, rotation, size);
        }

        public T Pool<T>(T poolable, Vector3 position, Quaternion rotation, float size = 1f) where T : IPoolable
        {
            poolable.Wake();
            poolable.Pool(position, rotation, size);

            return poolable;
        }
    }
}