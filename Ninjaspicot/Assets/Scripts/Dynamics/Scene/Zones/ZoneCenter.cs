using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Abstract;
using ZepLink.RiceNinja.Dynamics.Interfaces;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Zones
{
    public class ZoneCenter : SceneryElement, IPoolable
    {
        public override void Pool(Vector3 position, Quaternion rotation, float size = 1)
        {
            Transform.position = position;
        }
    }
}