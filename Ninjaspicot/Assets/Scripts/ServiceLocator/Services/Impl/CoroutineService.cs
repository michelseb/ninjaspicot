using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZepLink.RiceNinja.ServiceLocator.Services.Abstract;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class CoroutineService : GameService, ICoroutineService
    {
        private readonly IDictionary<Guid, Coroutine> _runningRoutines = new Dictionary<Guid, Coroutine>();
        private MonoBehaviour _coroutineServiceBehaviour;

        public override void Init(Transform parent)
        {
            base.Init(parent);

            _coroutineServiceBehaviour = ServiceObject.AddComponent<ServiceBehaviour>();
        }

        public bool IsCoroutineRunning(Guid id)
        {
            return _runningRoutines.ContainsKey(id);
        }

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return _coroutineServiceBehaviour.StartCoroutine(routine);
        }

        public Coroutine StartCoroutine(IEnumerator routine, out Guid id)
        {
            id = Guid.NewGuid();
            
            var coroutine = _coroutineServiceBehaviour.StartCoroutine(CoroutineWrapper(id, routine));

            if (coroutine == null)
                return default;

            _runningRoutines.Add(id, coroutine);

            return coroutine;
        }

        public void StopAllCoroutines()
        {
            foreach (var coroutine in _runningRoutines.Keys)
            {
                StopCoroutine(coroutine);
            }
        }

        public void StopCoroutine(Guid id)
        {
            if (!_runningRoutines.ContainsKey(id))
            {
                Debug.LogWarning($"Routine with id {id} could not be stopped because it was not declared");
                return;
            }

            _coroutineServiceBehaviour.StopCoroutine(_runningRoutines[id]);
            EndCoroutine(id);
        }

        private void EndCoroutine(Guid id)
        {
            if (!_runningRoutines.ContainsKey(id))
            {
                Debug.LogError($"Routine with id {id} could not be ended because it was not declared");
                return;
            }

            _runningRoutines.Remove(id);
        }

        private IEnumerator CoroutineWrapper(Guid id, IEnumerator coroutine)
        {
            yield return _coroutineServiceBehaviour.StartCoroutine(coroutine);

            EndCoroutine(id);
        }
    }
}
