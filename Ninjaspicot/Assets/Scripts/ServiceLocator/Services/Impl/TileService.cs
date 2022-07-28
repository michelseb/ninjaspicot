using UnityEngine;
using UnityEngine.Tilemaps;
using ZepLink.RiceNinja.Dynamics.Scenery.Map;
using ZepLink.RiceNinja.Dynamics.Scenery.Obstacles;
using ZepLink.RiceNinja.ServiceLocator.Services.Abstract;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class TileService : CollectionService<Dynamics.Scenery.Map.Tile>, ITileService
    {
        private readonly ITileBrushService _tileBrushService;
        private TileBrush _currentBrush;

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
                            typeof(Obstacle))
                        .GetComponent<Tilemap>();

                    _tileMap.orientation = Tilemap.Orientation.XY;
                    _tileMap.transform.SetParent(grid.transform);
                }

                return _tileMap;
            }
        }

        public TileService(ITileBrushService tileBrushService)
        {
            _tileBrushService = tileBrushService;
        }

        public void Generate(Texture2D map, BrushType brushType)
        {
            var sizeX = map.width;
            var sizeY = map.height;

            var brush = _tileBrushService.FindByBrushType(brushType);

            if (brush == null)
                return;

            for (var j = 0; j < sizeY; j++)
            {
                for (var i = 0; i < sizeX; i++)
                {
                    var color = map.GetPixel(i, j);
                    SetTile(new Vector3Int(i, j), brush, color);
                }
            }
        }

        private void SetTile(Vector3Int coords, TileBrush brush, Color color)
        {
            if (color != Color.black)
                return;

            Tilemap.SetTile(coords, brush.Tile);
        }
    }
}