using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Dynamics.Scenery.Map;
using ZepLink.RiceNinja.Helpers;
using ZepLink.RiceNinja.ServiceLocator.Services.Abstract;
using ZepLink.RiceNinja.Utils;

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
            if (map == null)
                return;

            var localizedColors = map
                .GetPixels32()
                .Select((c, i) => (color: c, index: i))
                .ToDictionary(x => PixelToCoord(x.index, map.width), x => x.color);

            foreach (var localizedColor in localizedColors)
            {
                GenerateAt(localizedColor.Key, localizedColor.Value);
            }
        }

        public void GenerateLights(Texture2D lightMap)
        {
            if (lightMap == null)
                return;

            var lights = ColorUtils.FindShapes(lightMap);

            foreach (var l in lights)
            {
                var pos = l[0];
                var light = new GameObject("zone", typeof(Light2D)).GetComponent<Light2D>();
                light.lightType = Light2D.LightType.Freeform;
                SetShapePath(light, l.Select(x => new Vector3(x.x, x.y)).ToArray());

                //light.transform.position = new Vector3(pos.x, pos.y);
            }
        }

        void SetFieldValue<T>(object obj, string name, T val)
        {
            var field = obj.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(obj, val);
        }

        void SetShapePath(Light2D light, Vector3[] path)
        {
            SetFieldValue(light, "m_ShapePath", path);
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
                typeof(PoolHelper).GetMethod(nameof(PoolHelper.PoolAt), new Type[] { typeof(Vector3), typeof(string) })
                    .MakeGenericMethod(brush.Instanciable.GetType())
                    .Invoke(null, new object[] { (Vector3)coords, brush.Instanciable.Name });
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