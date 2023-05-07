using UnityEngine;
using UnityEngine.Tilemaps;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.ServiceLocator;
using ZepLink.RiceNinja.ServiceLocator.Services;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Map
{
    [CreateAssetMenu(fileName = "Tile", menuName = "Zeplink/Tile")]
    public class TileObject : ScriptableObject, IDynamic
    {
        [SerializeField] private TileBase _tileModel;
        [SerializeField] private bool _canAttachTo;

        public int Id => GetInstanceID();
        public string Name => name;
        public ServiceFinder ServiceFinder => ServiceFinder.Instance;
        public Transform Transform => GetParent(null);
        public TileBase TileModel => _tileModel;
        public bool IsAttachable => _canAttachTo;

        public Transform GetParent(Transform parentZone)
        {
            return ServiceFinder.Get<ITileService>().Tilemap.transform;
        }
    }
}