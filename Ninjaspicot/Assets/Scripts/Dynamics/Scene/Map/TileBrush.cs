using UnityEngine;
using ZepLink.RiceNinja.Manageables;
using ZepLink.RiceNinja.ServiceLocator;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Map
{
    [CreateAssetMenu(fileName = "Tile Brush", menuName = "Brushes/Brush")]
    public class TileBrush : ScriptableObject, IManageable
    {
        [SerializeField] private string _name = "Brush";
        [SerializeField] private RuleTile _ruleTile;
        [SerializeField] private BrushType _type;

        public int Id => GetInstanceID();
        public string Name => _name;
        public BrushType Type => _type;
        public RuleTile Tile => _ruleTile;

        public ServiceFinder ServiceFinder => ServiceFinder.Instance;
    }
}