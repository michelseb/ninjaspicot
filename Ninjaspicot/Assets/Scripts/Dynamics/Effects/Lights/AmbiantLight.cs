using UnityEngine;
using UnityEngine.Rendering.Universal;
using ZepLink.RiceNinja.Dynamics.Scenery.Zones;
using ZepLink.RiceNinja.Interfaces;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Effects.Lights
{
    public class AmbiantLight : LightEffect, ISceneryWakeable, IResettable
    {
        protected Animator _animator;

        private Zone _zone;
        public Zone Zone { get { if (BaseUtils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

        private Color _initColor;
        private float _initIntensity;

        protected override void Awake()
        {
            base.Awake();

            _animator = GetComponent<Animator>();
            _light = GetComponent<Light2D>();
            _initColor = _light.color;
            _initIntensity = _light.intensity;
        }

        public override void Wake()
        {
            _animator.SetTrigger("Wake");
        }

        public override void Sleep()
        {
            _animator.SetTrigger("Sleep");
        }

        public override void DoReset()
        {
            _light.color = _initColor;
            _light.intensity = _initIntensity;
        }
    }
}