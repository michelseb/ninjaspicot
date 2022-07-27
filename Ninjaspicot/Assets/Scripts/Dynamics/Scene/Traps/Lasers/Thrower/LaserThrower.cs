using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Dynamics.Scenery.Zones;
using ZepLink.RiceNinja.Interfaces;
using ZepLink.RiceNinja.ServiceLocator.Services;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Traps.Lasers
{
    public class LaserThrower : Dynamic, ISceneryWakeable, IRaycastable, IResettable
    {
        private IPoolService _poolService;

        private Zone _zone;
        public Zone Zone { get { if (BaseUtils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

        protected virtual void Awake()
        {
            _poolService = ServiceFinder.Get<IPoolService>();
        }

        protected void OnTriggerEnter2D(Collider2D collision)

        {
            if (!collision.CompareTag("hero"))
                return;

            _poolService.GetPoolable<ThrownLaser>(Transform.position, Quaternion.identity);
        }

        public void Sleep()
        {
            gameObject.SetActive(false);
        }

        public void Wake()
        {
            gameObject.SetActive(true);
        }

        public void DoReset()
        {
        }
    }
}