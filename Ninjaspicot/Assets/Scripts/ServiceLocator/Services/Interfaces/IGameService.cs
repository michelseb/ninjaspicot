using UnityEngine;
using ZepLink.RiceNinja.Logger;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    /// <summary>
    /// Base interface for services searchable through ServiceLocator
    /// </summary>
    public interface IGameService
    {
        /// <summary>
        /// ServiceObject name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Object used for holding components 
        /// </summary>
        GameObject ServiceObject { get; }

        /// <summary>
        /// Criticity
        /// </summary>
        DebugMode DebugMode { get; }

        /// <summary>
        /// Initializes service
        /// </summary>
        /// /// <param name="parent"></param>
        void Init(Transform parent);

        /// <summary>
        /// Log event with object's criticity
        /// </summary>
        /// <param name="message"></param>
        void Log(string message);
    }
}