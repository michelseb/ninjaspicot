using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Manageables.Interfaces;
using ZepLink.RiceNinja.ServiceLocator;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Map
{
    [CreateAssetMenu(fileName = "Tile Brush", menuName = "Zeplink/Brush")]
    public class Brush : ScriptableObject, IManageable<Color>
    {
        [SerializeField] private Object _instanciableObject;
        [SerializeField] private Color _color;

        public Color Id => _color;
        public string Name => name;
        public Color Color => _color;

        private IDynamic _instanciable;
        public IDynamic Instanciable
        {
            get
            {
                if (BaseUtils.IsNull(_instanciable))
                {
                    if (BaseUtils.IsNull(_instanciableObject))
                    {
                        Debug.LogError($"No instanciable set for brush {Name}");
                        return null;
                    }
                    
                    if (_instanciableObject is not IDynamic instanciable)
                    {
                        if (_instanciableObject is not GameObject gameObject || !gameObject.TryGetComponent(out instanciable))
                        {
                            Debug.LogError($"Brush object {_instanciableObject.name} is not of type instanciable");
                            return null;
                        }
                    }

                    _instanciable = instanciable;
                }

                return _instanciable;
            }
        }

        public ServiceFinder ServiceFinder => ServiceFinder.Instance;
    }
}