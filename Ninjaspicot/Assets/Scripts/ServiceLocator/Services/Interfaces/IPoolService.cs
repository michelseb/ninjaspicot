using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface IPoolService : IGameService
    {
        /// <summary>
        /// Get poolable of type T at position with given rotation, size and parent
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="size"></param>
        /// <param name="parent"></param>
        /// <param name="defaultParent"></param>
        /// <returns></returns>
        T GetPoolable<T>(Vector3 position, Quaternion rotation, float size = 1f, Transform parent = null, bool defaultParent = true) where T : IPoolable;

        /// <summary>
        /// Pool poolable at position with rotation and size
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="poolable"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        T Pool<T>(T poolable, Vector3 position, Quaternion rotation, float size = 1f) where T : IPoolable;
    }
}