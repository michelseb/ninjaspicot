using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface IPoolService<T> : ICollectionService<int, T>, IPoolService where T : IPoolable
    {
        /// <summary>
        /// Parent of all instantiated objects
        /// </summary>
        Transform InstancesParent { get; }

        /// <summary>
        /// Returns model with given name
        /// </summary>
        /// <param name="name"></param>
        T GetModelByName(string name);

        /// <summary>
        /// Create instance of model at given position, rotation and parent
        /// </summary>
        /// <param name="model"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        T Create(T model, Vector3 position, Quaternion rotation);

        /// <summary>
        /// Create instance of model at given position
        /// </summary>
        /// <param name="model"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        T Create(T model, Vector3 position);

        /// <summary>
        /// Create instance of model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        T Create(T model);

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