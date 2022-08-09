using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.ServiceLocator;

namespace ZepLink.RiceNinja.Helpers
{
    public static class PoolHelper
    {
        public static T PoolAt<T>(Vector3 position) where T : IPoolable
        {
            return PoolAt<T>(position, Quaternion.identity);
        }

        public static T PoolAt<T>(Vector3 position, Quaternion rotation) where T : IPoolable
        {
            return PoolAt<T>(position, rotation, 1);
        }

        public static T PoolAt<T>(Vector3 position, Quaternion rotation, float size) where T : IPoolable
        {
            var service = ServiceFinder.Instance.GetPoolServiceFor<T>();

            return service.PoolAt(position, rotation, size);
        }
    }
}
