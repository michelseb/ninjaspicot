using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Abstract;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Dynamics.Scenery.Zones;
using ZepLink.RiceNinja.Helpers;
using ZepLink.RiceNinja.Interfaces;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Traps.Lasers
{
    public class LaserThrower : Dynamic, ISceneryWakeable, IRaycastable, IResettable
    {
        private Zone _zone;
        public Zone Zone { get { if (BaseUtils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

        protected void OnTriggerEnter2D(Collider2D collision)

        {
            if (!collision.CompareTag("hero"))
                return;

            PoolHelper.PoolAt<ThrownLaser>(Transform.position, Quaternion.identity);
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