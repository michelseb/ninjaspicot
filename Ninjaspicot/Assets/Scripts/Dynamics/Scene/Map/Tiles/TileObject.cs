using UnityEngine;
using UnityEngine.Tilemaps;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.ServiceLocator;
using ZepLink.RiceNinja.ServiceLocator.Services;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Map
{
    [CreateAssetMenu(fileName = "Tile", menuName = "Zeplink/Tile")]
    public class TileObject : ScriptableObject, IInstanciable
    {
        [SerializeField] private TileBase _tileModel;
        [SerializeField] private string _name;

        public int Id => GetInstanceID();
        public string Name => _name;
        public ServiceFinder ServiceFinder => ServiceFinder.Instance;
        public Transform Transform => throw new System.NotImplementedException();
        public Transform Parent => ServiceFinder.Get<ITileService>().Tilemap.transform;
        public TileBase TileModel => _tileModel;
    }
}