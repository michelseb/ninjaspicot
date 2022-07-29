using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Dynamics.Scenery.Obstacles;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Abstract
{
    public abstract class Physic : Dynamic, IPhysic
    {
        private Rigidbody2D _rigidbody;
        public Rigidbody2D Rigidbody { get { if (BaseUtils.IsNull(_rigidbody)) _rigidbody = GetComponent<Rigidbody2D>(); return _rigidbody; } }

        public abstract void LandOn(Obstacle obstacle, Vector3 contactPoint);
    }
}
