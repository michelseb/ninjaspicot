using System.Collections;
using UnityEngine;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface ICoroutineService : IGameService
    {
        /// <summary>
        /// Object used for holding components and 
        /// </summary>
        GameObject ServiceObject { get; }

        /// <summary>
        /// Behaviour attached to service object that can run coroutines
        /// </summary>
        MonoBehaviour CoroutineServiceBehaviour { get; }

        /// <summary>
        /// Run a coroutine on service behaviour and stops previous it if needed
        /// </summary>
        /// <param name="routine"></param>
        /// <param name="stopPrevious"></param>
        Coroutine StartCoroutine(IEnumerator routine, bool stopPrevious = true);

        /// <summary>
        /// Stops all coroutines of behaviour
        /// </summary>
        void StopAllCoroutines();

        /// <summary>
        /// Is coroutine with given name running
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool IsCoroutineRunning(string name);
    }
}