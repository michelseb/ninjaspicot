using UnityEngine;

namespace ZepLink.RiceNinja.Dynamics.Interfaces
{
    public interface IInstanciable : IDynamic
    {
        /// <summary>
        /// Instantiate as a child of this transform
        /// </summary>
        Transform Parent => null;
    }
}
