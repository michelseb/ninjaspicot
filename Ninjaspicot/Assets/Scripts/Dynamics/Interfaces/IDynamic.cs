using UnityEngine;
using ZepLink.RiceNinja.Manageables.Interfaces;

namespace ZepLink.RiceNinja.Dynamics.Interfaces
{
    public interface IDynamic : IManageable<int>
    {
        /// <summary>
        /// Gameobject transform
        /// </summary>
        Transform Transform { get; }
    }
}