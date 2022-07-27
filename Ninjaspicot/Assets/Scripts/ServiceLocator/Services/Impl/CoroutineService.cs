using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZepLink.RiceNinja.Manageables;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class CoroutineService<T> : CollectionService<T>, ICoroutineService where T : IManageable
    {
        public IDictionary<string, Coroutine> RunningRoutines { get; } = new Dictionary<string, Coroutine>();

        public MonoBehaviour CoroutineServiceBehaviour { get; private set; }

        public override void Init(Transform parent)
        {
            base.Init(parent);

            CoroutineServiceBehaviour = ServiceObject.AddComponent<ServiceBehaviour>();
        }

        public Coroutine StartCoroutine(IEnumerator routine, bool stopPrevious = true)
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

        private void StopCoroutine(string name)
        {
            if (!RunningRoutines.ContainsKey(name))
            {
                Debug.LogError($"Routine with name {name} could not be stopped because it was not declared");
                return;
            }

            CoroutineServiceBehaviour.StopCoroutine(RunningRoutines[name]);
            EndCoroutine(name);
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

        public void StopAllCoroutines()
        {
            foreach (var coroutine in RunningRoutines.Keys)
            {
                StopCoroutine(coroutine);
            }
        }

        public bool IsCoroutineRunning(string name)
        {
            return RunningRoutines.ContainsKey(name);
        }

        private IEnumerator CoroutineWrapper(string name, IEnumerator coroutine)
        {
            while (coroutine.MoveNext())
                yield return coroutine.Current;

            EndCoroutine(name);
        }
    }
}
