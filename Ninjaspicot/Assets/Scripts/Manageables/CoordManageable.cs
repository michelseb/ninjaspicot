using UnityEngine;

namespace ZepLink.RiceNinja.Manageables
{
    public abstract class CoordManageable : Manageable<Vector3Int>
    {
        public override Vector3Int Id { get; }
    }
}
