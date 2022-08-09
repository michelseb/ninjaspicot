using UnityEngine;
using UnityEngine.Tilemaps;
using ZepLink.RiceNinja.Dynamics.Scenery.Obstacles;
using ZepLink.RiceNinja.ServiceLocator.Services.Abstract;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class TileService : CollectionService<Vector3Int, Dynamics.Scenery.Map.Tile>, ITileService
    {
        private Tilemap _tileMap;
        public Tilemap Tilemap
        {
            get
            {
                if (BaseUtils.IsNull(_tileMap))
                {
                    var grid = new GameObject("Grid", typeof(Grid));
                    _tileMap = new GameObject("Level",
                            typeof(Tilemap),
                            typeof(TilemapRenderer),
                            typeof(TilemapCollider2D),
                            typeof(CompositeCollider2D),
                            typeof(Obstacle))
                        .GetComponent<Tilemap>();

                    _tileMap.GetComponent<Rigidbody2D>().isKinematic = true;

                    _tileMap.orientation = Tilemap.Orientation.XY;
                    _tileMap.transform.SetParent(grid.transform);
                }

                return _tileMap;
            }
        }

        public void SetTile(Vector3Int coords, TileBase tile)
        {
            Tilemap.SetTile(coords, tile);
        }
    }
}