using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Dynamics.Scenery.Zones;
using ZepLink.RiceNinja.Interfaces;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Utilities.Interactives
{
    public class AimableLocation : Dynamic, IFocusable, ISceneryWakeable
    {
        public bool IsSilent => true;

        public bool Active { get; set; }
        public bool Taken => !Active;

        private Zone _zone;
        public Zone Zone { get { if (BaseUtils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

        protected Animator _animator;
        protected SpriteRenderer _renderer;

        protected virtual void Awake()
        {
            _animator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
            _renderer = GetComponent<SpriteRenderer>();
        }

        public void Sleep()
        {
            _renderer.enabled = false;
            Active = false;
        }

        public void Wake()
        {
            _renderer.enabled = true;
            Active = true;
        }
    }
}
