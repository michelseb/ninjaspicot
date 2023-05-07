using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Dynamics.Scenery.Map;
using ZepLink.RiceNinja.Dynamics.Scenery.Obstacles;
using ZepLink.RiceNinja.ServiceLocator.Services.Abstract;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class TileService : CollectionService<Vector3Int, Dynamics.Scenery.Map.Tile>, ITileService
    {
        private ShadowCaster _caster;
        public ShadowCaster Caster { get { if (BaseUtils.IsNull(_caster)) _caster = Tilemap.GetComponent<ShadowCaster>(); return _caster; } }

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
                            typeof(CompositeShadowCaster2D),
                            typeof(ShadowCaster),
                            typeof(Obstacle))
                        .GetComponent<Tilemap>();

                    _tileMap.gameObject.isStatic = true;
                    _tileMap.gameObject.layer = LayerMask.NameToLayer("Obstacle");
                    _tileMap.GetComponent<Rigidbody2D>().isKinematic = true;

                    _tileMap.orientation = Tilemap.Orientation.XY;
                    _tileMap.transform.SetParent(grid.transform);

                    var collider = _tileMap.GetComponent<TilemapCollider2D>();

                    collider.usedByComposite = true;
                    collider.extrusionFactor = .1f;

                    var composite = _tileMap.GetComponent<CompositeCollider2D>();

                    composite.offsetDistance = .1f;
                }

                return _tileMap;
            }
        }

        public void SetTile(Vector3Int coords, TileBase tile, bool isAttachable)
        {
            Tilemap.SetTile(coords, tile);

            if (!isAttachable)
            {
                BuildDetacher(coords);
            }
        }

        public void GenerateShadows()
        {
            Caster.Generate();
        }

        private void BuildDetacher(Vector3Int coords)
        {
            var detacher = new GameObject("Detacher", typeof(Detacher));
            detacher.transform.position = (Vector3)coords + Vector3.one * .5f;
            detacher.transform.SetParent(Tilemap.transform);
            detacher.gameObject.isStatic = true;

            detacher.layer = LayerMask.NameToLayer("Trigger");

            var collider = detacher.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
        }
    }
}