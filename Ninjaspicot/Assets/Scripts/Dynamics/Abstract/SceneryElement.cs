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

        public virtual bool AlignedToWall => false;

        public virtual void DoReset()
        {
        }

        //public virtual void Start()
        //{
        //    if (AlignedToWall)
        //    {
        //        AlignToWall();
        //    }
        //}

        public virtual void Pool(Vector3 position, Quaternion rotation, float size = 1)
        {
            Transform.position = position;
            Transform.rotation = rotation;
            Transform.localScale *= size;

            //if (AlignedToWall)
            //{
            //    AlignToWall();
            //}
        }

        //private void AlignToWall()
        //{
        //    var direction = Vector2.right;
        //    for (var i = 0; i < 4; i++)
        //    {
        //        var cast = Physics2D.Raycast(Transform.position, direction, 10f, 1 << LayerMask.NameToLayer("Obstacle"));//CastUtils.RayCast(Transform.position, direction, 10f, layerMask: CastUtils.GetMask("Obstacle"));

        //        if (cast)
        //        {
        //            Transform.rotation = Quaternion.Euler(cast.point - BaseUtils.ToVector2(Transform.position));
        //            return;
        //        }

        //        direction = new Vector2(direction.y, -direction.x);
        //    }

        //}
    }
}
