using UnityEngine;
using UnityEngine.Rendering.Universal;
using ZepLink.RiceNinja.Dynamics.Scenery.Zones;
using ZepLink.RiceNinja.Interfaces;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Effects.Lights
{
    public class AmbiantLight : LightEffect, ISceneryWakeable, IResettable
    {
        //protected Animator _animator;
        //public Animator Animator
        //{
        //    get
        //    {
        //        if (BaseUtils.IsNull(_animator))
        //        {
        //            _animator = GetComponent<Animator>();
        //            _animator.runtimeAnimatorController = ServiceFinder.Get<IAnimationService>().FindByName("Zone").AnimatorController;
        //        }

        //        return _animator;
        //    }
        //}

        private Zone _zone;
        public Zone Zone { get { if (BaseUtils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

        private Color _initColor;
        private float _initIntensity;

        protected virtual void Start()
        {
            Light = GetComponent<Light2D>();
            _initColor = Light.color;
            _initIntensity = Light.intensity;
        }

        public override void Wake()
        {
            //Animator.SetTrigger("Open");
        }

        public override void Sleep()
        {
            //Animator.SetTrigger("Close");
        }

        public void Brighten()
        {
            //Animator.SetTrigger("Brighten");
        }

        public void Dimm()
        {
            //Animator.SetTrigger("Dimm");
        }

        public override void DoReset()
        {
            Light.color = _initColor;
            Light.intensity = _initIntensity;
        }
    }
}