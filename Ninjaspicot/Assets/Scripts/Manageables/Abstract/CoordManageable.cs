using UnityEngine;

namespace ZepLink.RiceNinja.Manageables.Abstract
{
    public abstract class CoordManageable : Manageable<Vector3Int>
    {
        public override Vector3Int Id { get; }
    }
}
