using System;
using UnityEngine;
using ZepLink.RiceNinja.Manageables.Interfaces;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface IComponentService : ICollectionService<int, IComponent>
    {
        /// <summary>
        /// Equips instance with component of type T
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        T Equip<T>(Transform instance) where T : class, IComponent;
    }
}