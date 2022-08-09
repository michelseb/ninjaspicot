using UnityEngine;
using ZepLink.RiceNinja.Manageables.Abstract;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Map
{
    public class Tile : CoordManageable
    {
        public Vector3Int Coords { get; private set; }
        public TileObject TileModel { get; private set; }
    }
}