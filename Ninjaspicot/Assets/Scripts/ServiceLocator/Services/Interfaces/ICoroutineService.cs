using System;
using System.Collections;
using UnityEngine;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface ICoroutineService : IGameService
    {
        /// <summary>
        /// Start a coroutine without keeping track of it
        /// </summary>
        /// <param name="routine"></param>
        /// <returns></returns>
        Coroutine StartCoroutine(IEnumerator routine);

        /// <summary>
        /// Run a coroutine on service behaviour and keep track of it
        /// </summary>
        /// <param name="routine"></param>
        Coroutine StartCoroutine(IEnumerator routine, out Guid id);

        /// <summary>
        /// Stops a running coroutine
        /// </summary>
        /// <param name="name"></param>
        void StopCoroutine(Guid id);

        /// <summary>
        /// Stops all coroutines of behaviour
        /// </summary>
        void StopAllCoroutines();

        /// <summary>
        /// Is coroutine with given id running
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool IsCoroutineRunning(Guid id);
    }
}