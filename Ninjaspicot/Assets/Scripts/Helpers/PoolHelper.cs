using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.ServiceLocator;

namespace ZepLink.RiceNinja.Helpers
{
    public static class PoolHelper
    {
        public static T Pool<T>(string modelName = default) where T : IPoolable
        {
            return PoolAt<T>(Vector3.zero, Quaternion.identity, modelName);
        }

        public static T PoolAt<T>(Vector3 position, string modelName = default) where T : IPoolable
        {
            return PoolAt<T>(position, Quaternion.identity, modelName);
        }

        public static T PoolAt<T>(Vector3 position, Quaternion rotation, string modelName = default) where T : IPoolable
        {
            return PoolAt<T>(position, rotation, 1, modelName);
        }

        public static T PoolAt<T>(Vector3 position, Quaternion rotation, float size, string modelName = default) where T : IPoolable
        {
            return ServiceFinder.Instance.PoolFor<T>(position, rotation, size, modelName);
        }
    }
}
