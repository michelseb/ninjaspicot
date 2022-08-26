using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Dynamics.Scenery.Zones;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Abstract
{
    public abstract class SceneryElement : Dynamic, IPoolable
    {
        private Zone _zone;
        public Zone Zone { get { if (BaseUtils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

        public virtual void DoReset()
        {
        }

        public virtual void Pool(Vector3 position, Quaternion rotation, float size = 1)
        {
            Transform.position = position;
            Transform.rotation = rotation;
            Transform.localScale *= size;
        }
    }
}
