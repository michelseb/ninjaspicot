using UnityEngine;
using ZepLink.RiceNinja.Interfaces;

namespace ZepLink.RiceNinja.Dynamics.Interfaces
{
    public interface IPoolable : IInstanciable, IWakeable, IResettable
    {
        /// <summary>
        /// Pools poolable at given position, rotation and size
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="size"></param>
        void Pool(Vector3 position, Quaternion rotation, float size = 1);
    }
}
