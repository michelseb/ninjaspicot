using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.ServiceLocator;

namespace ZepLink.RiceNinja.Helpers
{
    public static class PoolHelper
    {
        public static T Pool<T>(string modelName = default, Transform zone = default) where T : IPoolable
        {
            return Pool<T>(Vector3.zero, Quaternion.identity, modelName, zone);
        }

        public static T Pool<T>(Vector3 position, string modelName = default) where T : IPoolable
        {
            return Pool<T>(position, Quaternion.identity, modelName, null);
        }

        public static T Pool<T>(Vector3 position, string modelName = default, Transform zone = default) where T : IPoolable
        {
            return Pool<T>(position, Quaternion.identity, modelName, zone);
        }

        public static T Pool<T>(Vector3 position, Quaternion rotation, string modelName = default, Transform zone = default) where T : IPoolable
        {
            return Pool<T>(position, rotation, 1, modelName, zone);
        }

        public static T Pool<T>(Vector3 position, Quaternion rotation, float size, string modelName = default, Transform zone = default) where T : IPoolable
        {
            return ServiceFinder.Instance.PoolFor<T>(position, rotation, size, modelName, zone);
        }
    }
}
