using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Scenery.Map;
using ZepLink.RiceNinja.ServiceLocator.Services.Abstract;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class MapService : GeneratorService, IMapService
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
            if (map == null)
                return;

            var localizedColors = GetLocalizedColors(map);

            foreach (var localizedColor in localizedColors)
            {
                GenerateAt(localizedColor.Key, localizedColor.Value);
            }
        }

        private Dictionary<PoolSetting, Color32> GetLocalizedColors(Texture2D map)
        {
            return map
                .GetPixels32()
                .Select((c, i) => (color: c, index: i))
                .Where(x => x.color.a > 0)
                .ToDictionary(x => new PoolSetting(PixelToCoord(x.index, map.width), Quaternion.identity), x => x.color);
        }

        protected override void GenerateAt(PoolSetting setting, Color color, Transform zone = default)
        {
            var brush = _brushService.FindById(color);

            if (brush == null)
                return;

            if (brush.Instanciable is not TileObject tileObject)
                return;

            _tileService.SetTile(setting.Position, tileObject.TileModel, tileObject.IsAttachable);
        }
    }
}