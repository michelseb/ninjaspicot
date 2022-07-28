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
    }
}
