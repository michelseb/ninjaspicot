using UnityEngine;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    /// <summary>
    /// Base interface for services searchable through ServiceLocator
    /// </summary>
    public interface IGameService
    {
        /// <summary>
        /// Initializes service
        /// </summary>
        /// /// <param name="parent"></param>
        void Init(Transform parent);
    }
}