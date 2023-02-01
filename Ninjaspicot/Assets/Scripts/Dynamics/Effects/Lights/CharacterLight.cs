using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Scenery.Zones;
using ZepLink.RiceNinja.Interfaces;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Effects.Lights
{
    public class CharacterLight : LightEffect, ISceneryWakeable
    {
        protected Animator _animator;

        private Zone _zone;
        public Zone Zone { get { if (BaseUtils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

        protected virtual void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        protected virtual void LateUpdate()
        {
            Transform.rotation = Quaternion.identity;
        }

        public override void Wake()
        {
            _animator.SetTrigger("TurnOn");
        }

        public override void Sleep()
        {
            _animator.SetTrigger("TurnOff");
        }
    }
}