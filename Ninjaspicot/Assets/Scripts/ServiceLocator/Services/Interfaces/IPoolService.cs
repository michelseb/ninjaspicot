using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface IPoolService<T> : IInstanceService<T>, IPoolService where T : IPoolable
    {
        /// <summary>
        /// Pool poolable at position with rotation and size
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        T PoolAt(Vector3 position, Quaternion rotation, float size, string modelName = default);

        /// <summary>
        /// Pool poolable at position and rotation
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        T PoolAt(Vector3 position, Quaternion rotation, string modelName = default);

        /// <summary>
        /// Pool poolable at position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        T PoolAt(Vector3 position, string modelName = default);
    }

    public interface IPoolService : IGameService
    {
        /// <summary>
        /// Pool poolable at position with rotation and size
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        IPoolable Pool(Vector3 position, Quaternion rotation, float size, string modelName = default);

        /// <summary>
        /// Pool poolable at position and rotation
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        IPoolable Pool(Vector3 position, Quaternion rotation, string modelName = default);

        /// <summary>
        /// Pool poolable at position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        IPoolable Pool(Vector3 position, string modelName = default);
    }
}