using UnityEngine;
using ZepLink.RiceNinja.Manageables;

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