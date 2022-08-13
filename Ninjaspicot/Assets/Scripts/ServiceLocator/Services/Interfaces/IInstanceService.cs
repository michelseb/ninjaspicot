using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface IInstanceService<T> : ICollectionService<int, T>, IGameService where T : IDynamic
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
    }
}