using UnityEngine;
using UnityEngine.Tilemaps;
using ZepLink.RiceNinja.Manageables;
using ZepLink.RiceNinja.ServiceLocator;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Map
{
    [CreateAssetMenu(fileName = "Tile Brush", menuName = "Brushes/Brush")]
    public class TileBrush : ScriptableObject, IManageable<Color>
    {
        [SerializeField] private string _name = "Brush";
        [SerializeField] private TileBase _tile;
        [SerializeField] private BrushType _type;
        [SerializeField] private Color _color;

        public Color Id => _color;
        public string Name => _name;
        public BrushType Type => _type;
        public TileBase Tile => _tile;
        public Color Color => _color;

        public ServiceFinder ServiceFinder => ServiceFinder.Instance;
    }
}