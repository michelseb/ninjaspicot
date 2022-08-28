using System;
using System.Collections.Generic;
using UnityEngine;
using ZepLink.RiceNinja.Manageables.Interfaces;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Abstract
{
    public class CoroutineService<T, U> : CollectionService<T, U>, ICoroutineService where U : IManageable<T>
    {
        public IDictionary<Guid, Coroutine> RunningRoutines { get; } = new Dictionary<Guid, Coroutine>();

        public MonoBehaviour CoroutineServiceBehaviour { get; private set; }

        public override void Init(Transform parent)
        {
            base.Init(parent);

            CoroutineServiceBehaviour = ServiceObject.AddComponent<ServiceBehaviour>();
        }
    }
}
