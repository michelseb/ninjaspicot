using UnityEngine;
using UnityEngine.Tilemaps;
using ZepLink.RiceNinja.Manageables;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Map
{
    public class Tile : CoordManageable
    {
        public Vector3Int Coords { get; private set; }
        public TileBase TileModel { get; private set; }
    }
}