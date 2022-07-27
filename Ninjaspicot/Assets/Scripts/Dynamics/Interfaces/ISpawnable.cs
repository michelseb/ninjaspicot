using UnityEngine;

namespace ZepLink.RiceNinja.Dynamics.Interfaces
{
    public interface ISpawnable : IDynamic
    {
        /// <summary>
        /// initializes ISpawnable's spawn
        /// </summary>
        void InitSpawn();
    }
}