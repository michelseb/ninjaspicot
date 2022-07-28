using System.Collections;
using System.Collections.Generic;
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
        /// Coroutines that are currently running
        /// </summary>
        IDictionary<string, Coroutine> RunningRoutines { get; }

        /// <summary>
        /// Behaviour attached to service object that can run coroutines
        /// </summary>
        MonoBehaviour CoroutineServiceBehaviour { get; }

        /// <summary>
        /// Run a coroutine on service behaviour and stops previous it if needed
        /// </summary>
        /// <param name="routine"></param>
        /// <param name="stopPrevious"></param>
        Coroutine StartCoroutine(IEnumerator routine, bool stopPrevious = true)
        {
            var name = nameof(routine);

            if (stopPrevious && RunningRoutines.ContainsKey(name))
            {
                StopCoroutine(name);
            }

            var coroutine = CoroutineServiceBehaviour.StartCoroutine(CoroutineWrapper(name, routine));
            RunningRoutines.Add(name, coroutine);

            return coroutine;
        }

        /// <summary>
        /// Stops a running coroutine
        /// </summary>
        /// <param name="name"></param>
        void StopCoroutine(string name)
        {
            if (!RunningRoutines.ContainsKey(name))
            {
                Debug.LogError($"Routine with name {name} could not be stopped because it was not declared");
                return;
            }

            CoroutineServiceBehaviour.StopCoroutine(RunningRoutines[name]);
            EndCoroutine(name);
        }

        /// <summary>
        /// Stops all coroutines of behaviour
        /// </summary>
        void StopAllCoroutines()
        {
            foreach (var coroutine in RunningRoutines.Keys)
            {
                StopCoroutine(coroutine);
            }
        }

        /// <summary>
        /// Is coroutine with given name running
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool IsCoroutineRunning(string name)
        {
            return RunningRoutines.ContainsKey(name);
        }

        private void EndCoroutine(string name)
        {
            if (!RunningRoutines.ContainsKey(name))
            {
                Debug.LogError($"Routine with name {name} could not be ended because it was not declared");
                return;
            }

            RunningRoutines.Remove(name);
        }

        private IEnumerator CoroutineWrapper(string name, IEnumerator coroutine)
        {
            while (coroutine.MoveNext())
                yield return coroutine.Current;

            EndCoroutine(name);
        }
    }
}