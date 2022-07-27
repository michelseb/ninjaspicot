using UnityEngine;

namespace ZepLink.RiceNinja.Dynamics.Interfaces
{
    public interface IShootable : IKillable
    {
        /// <summary>
        /// Is it visible from a given position
        /// </summary>
        bool IsVisibleFrom(Vector3 position, int layerMask = 0);
    }
}