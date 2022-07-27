using System;
using UnityEngine;
using ZepLink.RiceNinja.Manageables;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface IInstantiatorService<T> : ICollectionService<T> where T : IManageable
    {
        /// <summary>
        /// Returns a random non-instantiated model from the list of assets of given type
        /// </summary>
        T GetRandomModel();

        /// <summary>
        /// Returns model with given name
        /// </summary>
        /// <param name="name"></param>
        T GetModelByName(string name);

        /// <summary>
        /// Returns a random instantiated object of given type
        /// </summary>
        T GetRandomInstance();

        /// <summary>
        /// Instantiates an object from given model and stores it into _instances
        /// </summary>
        T Create(T model, Vector3 position, Quaternion rotation, Transform parent = default);

        /// <summary>
        /// Instantiates an object from given model at position and identity rotation and stores it into _instances
        /// </summary>
        T Create(T model, Vector3 position, Transform parent = default);

        /// <summary>
        /// Instantiates an object from given model and stores it into _instances
        /// </summary>
        T Create(T model, Transform parent = default);

        /// <summary>
        /// Equips instance with component of type T
        /// </summary>
        /// <param name="componentType"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        T Equip(Type componentType, Transform instance);
    }
}