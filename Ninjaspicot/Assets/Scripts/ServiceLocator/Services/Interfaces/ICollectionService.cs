using System.Collections.Generic;
using ZepLink.RiceNinja.Manageables;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface ICollectionService<T> : IGameService where T : IManageable
    {   
        /// <summary>
        /// References stored by id (GUID) in a dictionary
        /// </summary>
        IDictionary<int, T> InstancesDictionary { get; }

        /// <summary>
        /// List of instances of T
        /// </summary>
        IList<T> Collection { get; }

        /// <summary>
        /// Add instance to collection
        /// </summary>
        /// <param name="instance"></param>
        void Add(T instance);
        
        /// <summary>
        /// Remove instance from collection by id
        /// </summary>
        /// <param name="id"></param>
        void Remove(int id);

        /// <summary>
        /// Get object with given id
        /// </summary>
        T FindById(int id);

        /// <summary>
        /// Get object with given name
        /// </summary>
        T FindByName(string name);
    }
}