using System;
using System.Linq;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Scenery.Map;
using ZepLink.RiceNinja.Helpers;
using ZepLink.RiceNinja.ServiceLocator.Services.Abstract;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class MapService : GameService, IMapService
    {
        private readonly IBrushService _brushService;
        private readonly ITileService _tileService;

        public MapService(IBrushService brushService, ITileService tileService)
        {
            _brushService = brushService;
            _tileService = tileService;
        }

        public void Generate(Texture2D map)
        {
            var localizedColors = map
                .GetPixels32()
                .Select((c, i) => (color: c, index: i))
                .ToDictionary(x => PixelToCoord(x.index, map.width), x => x.color);

            foreach (var localizedColor in localizedColors)
            {
                GenerateAt(localizedColor.Key, localizedColor.Value);
            }
        }

        private void GenerateAt(Vector3Int coords, Color color)
        {
            var brush = _brushService.FindById(color);

            if (brush == null)
                return;

            if (brush.Instanciable.GetType().IsSubclassOf(typeof(MonoBehaviour)))
            {
                //typeof(PoolService).GetMethod(nameof(PoolService.PoolAt), new Type[] { typeof(Vector3) })
                //    .MakeGenericMethod(brush.Instanciable.GetType())
                //    .Invoke(_poolService, new object[] { (Vector3)coords });
                var type = brush.Instanciable.GetType().FullName;
                typeof(PoolHelper).GetMethod(nameof(PoolHelper.PoolAt), new Type[] { typeof(Vector3) })
                    .MakeGenericMethod(brush.Instanciable.GetType())
                    .Invoke(null, new object[] { (Vector3)coords });
            }
            else if (brush.Instanciable is TileObject tileObject)
            {
                _tileService.SetTile(coords, tileObject.TileModel);
            }
        }

        private Vector3Int PixelToCoord(int index, int imgWidth)
        {
            var x = index % imgWidth;
            var y = index / imgWidth;

            return new Vector3Int(x, y);
        }
    }
}