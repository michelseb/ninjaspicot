using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Abstract;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Dynamics.Scenery.Traps.Lasers;
using ZepLink.RiceNinja.Dynamics.Scenery.Zones;
using ZepLink.RiceNinja.Interfaces;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Utilities.Interactives
{
    public class Generator : Dynamic, IActivable, ISceneryWakeable, IRaycastable, IFocusable, IResettable
    {
        [SerializeField] protected Laser[] _lasers;

        private Zone _zone;
        public Zone Zone { get { if (BaseUtils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

        public bool IsSilent => false;
        public bool Taken => false;

        private Lamp _light;

        private void Awake()
        {
            _light = GetComponentInChildren<Lamp>();
        }

        public void Activate(IActivator activator = default)
        {
            Sleep();

            foreach (var laser in _lasers)
            {
                laser.Deactivate();
            }
        }

        public void Deactivate(IActivator activator = default) { }

        public void DoReset()
        {
            Wake();

            foreach (var laser in _lasers)
            {
                laser.DoReset();
            }
        }

        public void Sleep()
        {
            _light.Sleep();
        }

        public void Wake()
        {
            _light.Wake();
        }
    }
}