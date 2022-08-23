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

        private Dictionary<PoolSetting, Color32> GetLocalizedColors(Texture2D map)
        {
            return map
                .GetPixels32()
                .Select((c, i) => (color: c, index: i))
                .Where(x => x.color.a > 0)
                .ToDictionary(x => new PoolSetting(PixelToCoord(x.index, map.width), Quaternion.identity), x => x.color);
        }

        private Dictionary<PoolSetting, Color> GetLocalizedColors(Texture2D map, Texture2D terrain, int startX, int startY, int width, int height)
        {

            var offsetIndex = startX + startY * map.width;
            var offset = PixelToCoord(offsetIndex, map.width);

            return map
                .GetPixels(startX, startY, width, height)
                .Select((c, i) => (color: c, index: i))
                .Where(x => x.color.a > 0)
                .ToDictionary(x => new PoolSetting(PixelToCoord(x.index, width) + offset, GetTerrainAlignment(x.index, offsetIndex, width, terrain)), x => x.color);
        }

        public void GenerateZones(Texture2D zoneMap, Texture2D utilitiesMap, Texture2D structureMap)
        {
            if (zoneMap == null)
                return;

            var zones = ColorUtils.FindShapes(zoneMap);

            foreach (var z in zones)
            {
                var zoneObject = new GameObject("zone", typeof(Animator), typeof(Light2D), typeof(PolygonCollider2D), typeof(Zone));
                var zone = zoneObject.GetComponent<Zone>();
                _zoneService.Add(zone);

                var startX = z.Min(x => x.x);
                var startY = z.Min(x => x.y);

                var middle = new Vector2(z.Average(x => (float)x.x), z.Average(x => (float)x.y));

                var width = z.Max(x => x.x) + 1 - startX;
                var height = z.Max(x => x.y) + 1 - startY;

                var utilities = GetLocalizedColors(utilitiesMap, structureMap, startX, startY, width, height);

                foreach (var utility in utilities)
                {
                    GenerateAt(utility.Key, utility.Value, zone.Transform);
                }

                var collider = zoneObject.GetComponent<PolygonCollider2D>();
                collider.isTrigger = true;
                collider.points = z.Select(x => new Vector2(x.x, x.y) + new Vector2(1.5f * Mathf.Sign(x.x - middle.x), 1.5f * Mathf.Sign(x.y - middle.y))/*(new Vector2(x.x, x.y) - middle).normalized * 3*/).ToArray();

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

        private void GenerateAt(PoolSetting setting, Color color, Transform zone = default)
        {
            var brush = _brushService.FindById(color);

            if (brush == null)
                return;

            if (brush.Instanciable.GetType().IsSubclassOf(typeof(MonoBehaviour)))
            {
                var type = brush.Instanciable.GetType().FullName;

                typeof(PoolHelper).GetMethod(nameof(PoolHelper.Pool), new Type[] { typeof(Vector3), typeof(Quaternion), typeof(string), typeof(Transform) })
                    .MakeGenericMethod(brush.Instanciable.GetType())
                    .Invoke(null, new object[] { (Vector3)setting.Position, setting.Rotation, brush.Instanciable.Name, zone });
            }
            else if (brush.Instanciable is TileObject tileObject)
            {
                _tileService.SetTile(setting.Position, tileObject.TileModel);
            }
        }

        private Vector3Int PixelToCoord(int index, int imgWidth)
        {
            var x = index % imgWidth;
            var y = index / imgWidth;

            return new Vector3Int(x, y);
        }

        private Quaternion GetTerrainAlignment(int index, int offset, int width, Texture2D structureMap)
        {
            var structureMapWidth = structureMap.width;
            var realIndex = offset + index % width + structureMapWidth * (index / width);

            var coords = PixelToCoord(realIndex, structureMapWidth);
            var direction = Vector2Int.right;

            for (var i = 0; i < 4; i++)
            {
                if (structureMap.GetPixel(coords.x + direction.x, coords.y + direction.y) == Color.black)
                    return Quaternion.LookRotation(Vector3.forward, (Vector2)direction);

                direction = new Vector2Int(direction.y, -direction.x);
            }

            return Quaternion.identity;
        }

        public class PoolSetting
        {
            public Vector3Int Position { get; private set; }
            public Quaternion Rotation { get; private set; }

            public PoolSetting(Vector3Int position, Quaternion rotation)
            {
                Position = position;
                Rotation = rotation;
            }
        }
    }
}