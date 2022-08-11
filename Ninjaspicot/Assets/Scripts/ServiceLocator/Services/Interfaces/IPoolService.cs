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
        T PoolAt(Vector3 position, Quaternion rotation, float size, T model = default);

        /// <summary>
        /// Pool poolable at position and rotation
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        T PoolAt(Vector3 position, Quaternion rotation, T model = default);

        /// <summary>
        /// Pool poolable at position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        T PoolAt(Vector3 position, T model = default);

        /// <summary>
        /// Pool first poolable matching given name at position with rotation and size
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        T PoolByName(string name, Vector3 position, Quaternion rotation, float size);

        /// <summary>
        /// Pool first poolable matching given name at position and rotation
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        T PoolByName(string name, Vector3 position, Quaternion rotation);

        /// <summary>
        /// Pool first poolable matching given name at position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        T PoolByName(string name, Vector3 position);
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
        IPoolable Pool(Vector3 position, Quaternion rotation, float size, IPoolable model);

        /// <summary>
        /// Pool poolable at position and rotation
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        IPoolable Pool(Vector3 position, Quaternion rotation, IPoolable model);

        /// <summary>
        /// Pool poolable at position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        IPoolable Pool(Vector3 position, IPoolable model);
    }
}