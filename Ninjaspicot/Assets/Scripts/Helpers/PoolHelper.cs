using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.ServiceLocator;

namespace ZepLink.RiceNinja.Helpers
{
    public static class PoolHelper
    {
        public static T Pool<T>(IPoolable model = default) where T : IPoolable
        {
            return PoolAt<T>(Vector3.zero, Quaternion.identity, model);
        }

        public static T PoolAt<T>(Vector3 position, IPoolable model = default) where T : IPoolable
        {
            return PoolAt<T>(position, Quaternion.identity, model);
        }

        public static T PoolAt<T>(Vector3 position, Quaternion rotation, IPoolable model = default) where T : IPoolable
        {
            return PoolAt<T>(position, rotation, 1, model);
        }

        public static T PoolAt<T>(Vector3 position, Quaternion rotation, float size, IPoolable model = default) where T : IPoolable
        {
            return ServiceFinder.Instance.PoolFor<T>(position, rotation, size, model);
        }
    }
}
