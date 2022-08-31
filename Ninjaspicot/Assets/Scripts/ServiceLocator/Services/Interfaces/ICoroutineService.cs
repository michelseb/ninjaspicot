using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface ICoroutineService : IGameService
    {
        /// <summary>
        /// Coroutines that are currently running
        /// </summary>
        IDictionary<Guid, Coroutine> RunningRoutines { get; }

        /// <summary>
        /// Behaviour attached to service object that can run coroutines
        /// </summary>
        MonoBehaviour CoroutineServiceBehaviour { get; }

        /// <summary>
        /// Run a coroutine on service behaviour and stops previous it if needed
        /// </summary>
        /// <param name="routine"></param>
        Guid StartCoroutine(IEnumerator routine)
        {
            var key = Guid.NewGuid();

            var coroutine = CoroutineServiceBehaviour.StartCoroutine(CoroutineWrapper(key, routine));
            RunningRoutines.Add(key, coroutine);

            return key;
        }

        /// <summary>
        /// Stops a running coroutine
        /// </summary>
        /// <param name="name"></param>
        void StopCoroutine(Guid id)
        {
            if (!RunningRoutines.ContainsKey(id))
            {
                Debug.LogError($"Routine with id {id} could not be stopped because it was not declared");
                return;
            }

            CoroutineServiceBehaviour.StopCoroutine(RunningRoutines[id]);
            EndCoroutine(id);
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
        /// Is coroutine with given id running
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool IsCoroutineRunning(Guid id)
        {
            return RunningRoutines.ContainsKey(id);
        }

        /// <summary>
        /// Execute action with tiny delay (.01 sec)
        /// </summary>
        /// <param name="action"></param>
        void ExecuteNextFrame(Action action)
        {
            CoroutineServiceBehaviour.Invoke(action.Method.Name, .01f);
        }

        private void EndCoroutine(Guid id)
        {
            if (!RunningRoutines.ContainsKey(id))
            {
                Debug.LogError($"Routine with id {id} could not be ended because it was not declared");
                return;
            }

            RunningRoutines.Remove(id);
        }

        private IEnumerator CoroutineWrapper(Guid id, IEnumerator coroutine)
        {
            while (coroutine.MoveNext())
                yield return coroutine.Current;

            EndCoroutine(id);
        }
    }
}