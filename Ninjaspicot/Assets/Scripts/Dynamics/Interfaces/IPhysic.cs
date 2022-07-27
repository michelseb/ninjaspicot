using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Scenery.Obstacles;

namespace ZepLink.RiceNinja.Dynamics.Interfaces
{
    public interface IPhysic : IDynamic
    {
        /// <summary>
        /// Object rigidbody
        /// </summary>
        Rigidbody2D Rigidbody { get; }

        /// <summary>
        /// Landing behaviour
        /// </summary>
        /// <param name="obstacle"></param>
        /// <param name="contactPoint"></param>
        void LandOn(Obstacle obstacle, Vector3 contactPoint);
    }
}