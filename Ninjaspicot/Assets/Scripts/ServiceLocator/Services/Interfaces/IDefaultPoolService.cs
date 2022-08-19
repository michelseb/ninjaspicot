using System;
using System.Collections.Generic;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface IDefaultPoolService : IPoolService<IPoolable>
    {
        /// <summary>
        /// All poolables grouped by poolable type
        /// </summary>
        IDictionary<Type, List<IPoolable>> Poolables { get; }

        /// <summary>
        /// Pool poolable of given type at position with rotation and size
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="size"></param>
        /// <param name="modelName"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        IPoolable PoolAt(Vector3 position, Quaternion rotation, float size, Type poolableType, string modelName = default, Transform zone = default);
    }
}