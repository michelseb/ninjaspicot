using System.Collections.Generic;
using ZepLink.RiceNinja.Manageables.Interfaces;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface ICollectionService<T, U> : ICollectionService, IGameService where U : IManageable<T>
    {   
        /// <summary>
        /// References stored by id (GUID) in a dictionary
        /// </summary>
        IDictionary<T, U> InstancesDictionary { get; }

        /// <summary>
        /// List of instances of T
        /// </summary>
        IList<U> Collection { get; }

        /// <summary>
        /// Add instance to collection
        /// </summary>
        /// <param name="instance"></param>
        void Add(U instance);
        
        /// <summary>
        /// Remove instance from collection by id
        /// </summary>
        /// <param name="id"></param>
        void Remove(T id);

        /// <summary>
        /// Get object with given id
        /// </summary>
        U FindById(T id);

        /// <summary>
        /// Get object with given name
        /// </summary>
        U FindByName(string name);
    }

    public interface ICollectionService : IGameService
    {
        /// <summary>
        /// Add instance to collection
        /// </summary>
        /// <param name="instance"></param>
        void AddBase(IManageable instance);

        /// <summary>
        /// Remove instance from collection by id
        /// </summary>
        /// <param name="id"></param>
        void RemoveBase(IManageable id);
    }
}