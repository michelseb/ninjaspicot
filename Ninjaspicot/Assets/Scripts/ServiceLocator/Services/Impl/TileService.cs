using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using ZepLink.RiceNinja.Dynamics.Scenery.Map;
using ZepLink.RiceNinja.Dynamics.Scenery.Obstacles;
using ZepLink.RiceNinja.ServiceLocator.Services.Abstract;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class TileService : CollectionService<Vector3Int, Dynamics.Scenery.Map.Tile>, ITileService
    {
        private readonly ITileBrushService _tileBrushService;

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
            //var brush = _tileBrushService.FindByBrushType(brushType);

            //if (brush == null)
            //    return;

            var mapColors = map
                .GetPixels32()
                .Select((c, i) => (color: c, index: i))
                .ToDictionary(x => PixelToCoord(x.index, map.width), x => x.color);

            foreach (var color in mapColors)
            {
                SetTile(color.Key, color.Value);
            }

            //for (var j = 0; j < sizeY; j++)
            //{
            //    for (var i = 0; i < sizeX; i++)
            //    {
            //        map.GetP
            //        var color = map.GetPixel(i, j);
            //        SetTile(new Vector3Int(i, j), brush, color);
            //    }
            //}
        }

        private void SetTile(Vector3Int coords, Color color)
        {
            var brush = _tileBrushService.FindById(color);

            if (brush == null)
                return;

            Add(new Dynamics.Scenery.Map.Tile());

            Tilemap.SetTile(coords, brush.Tile);
        }

        private Vector3Int PixelToCoord(int index, int imgWidth)
        {
            var x = index % imgWidth;
            var y = index / imgWidth;

            return new Vector3Int(x, y);
        }
    }
}