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

        /// <summary>
        /// Name of parent gameObject
        /// </summary>
        string ParentName => string.Empty;

        /// <summary>
        /// Instantiate as a child of this transform
        /// </summary>
        Transform Parent { get; }
    }
}