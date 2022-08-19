using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using ZepLink.RiceNinja.Dynamics.Scenery.Map;
using ZepLink.RiceNinja.Dynamics.Scenery.Zones;
using ZepLink.RiceNinja.Helpers;
using ZepLink.RiceNinja.ServiceLocator.Services.Abstract;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class MapService : GameService, IMapService
    {
        private readonly IBrushService _brushService;
        private readonly ITileService _tileService;
        private readonly IZoneService _zoneService;

        public MapService(IBrushService brushService, ITileService tileService, IZoneService zoneService)
        {
            _brushService = brushService;
            _tileService = tileService;
            _zoneService = zoneService;
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

        private Dictionary<Vector3Int, Color32> GetLocalizedColors(Texture2D map)
        {
            return map
                .GetPixels32()
                .Select((c, i) => (color: c, index: i))
                .Where(x => x.color.a > 0)
                .ToDictionary(x => PixelToCoord(x.index, map.width), x => x.color);
        }

        private Dictionary<Vector3Int, Color> GetLocalizedColors(Texture2D map, int startX, int startY, int width, int height)
        {

            var offsetIndex = startX + startY * map.width;
            var offset = PixelToCoord(offsetIndex, map.width);

            return map
                .GetPixels(startX, startY, width, height)
                .Select((c, i) => (color: c, index: i))
                .Where(x => x.color.a > 0)
                .ToDictionary(x => PixelToCoord(x.index, width) + offset, x => x.color);
        }

        public void GenerateZones(Texture2D zoneMap, Texture2D utilitiesMap)
        {
            if (zoneMap == null)
                return;

            var zones = ColorUtils.FindShapes(zoneMap);

            foreach (var z in zones)
            {
                var zoneObject = new GameObject("zone", typeof(Animator), typeof(Light2D), typeof(Zone));
                var zone = zoneObject.GetComponent<Zone>();
                _zoneService.Add(zone);

                var startX = z.Min(x => x.x);
                var startY = z.Min(x => x.y);

                var width = z.Max(x => x.x) - startX;
                var height = z.Max(x => x.y) - startY;

                var utilities = GetLocalizedColors(utilitiesMap, startX, startY, width, height);

                foreach (var utility in utilities)
                {
                    GenerateAt(utility.Key, utility.Value, zone.Transform);
                }

                var light = zoneObject.GetComponent<Light2D>();
                light.lightType = Light2D.LightType.Freeform;
                light.intensity = 2f;
                light.color = ColorUtils.NightBlue;

                SetFieldValue(light, "m_ShapePath", z.Select(x => new Vector3(x.x, x.y)).ToArray());
                SetFieldValue(light, "m_ShapeLightFalloffSize", 2f);

                light.transform.position = Vector3.one * .5f;
            }
        }

        void SetFieldValue<T>(object obj, string name, T val)
        {
            var field = obj.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(obj, val);
        }

        private void GenerateAt(Vector3Int coords, Color color, Transform zone = default)
        {
            var brush = _brushService.FindById(color);

            if (brush == null)
                return;

            if (brush.Instanciable.GetType().IsSubclassOf(typeof(MonoBehaviour)))
            {
                var type = brush.Instanciable.GetType().FullName;

                typeof(PoolHelper).GetMethod(nameof(PoolHelper.Pool), new Type[] { typeof(Vector3), typeof(string), typeof(Transform) })
                    .MakeGenericMethod(brush.Instanciable.GetType())
                    .Invoke(null, new object[] { (Vector3)coords, brush.Instanciable.Name, zone });
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