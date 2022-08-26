using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Abstract;
using ZepLink.RiceNinja.Dynamics.Effects.Lights;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Interfaces;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Utilities.Interactives
{
    public class Lamp : SceneryElement, ISceneryWakeable, IFocusable, IActivable, IResettable, IPoolable
    {
        public Animator Animator { get; private set; }

        protected LightEffect _light;

        public bool StayOn { get; set; }
        public bool IsSilent => false;
        public bool Taken => false;

        protected bool _broken;


        protected virtual void Awake()
        {
            Animator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
            _light = GetComponent<LightEffect>();
        }

        public virtual void Wake()
        {
            if (_broken)
                return;

            Animator.SetTrigger("TurnOn");
        }

        public virtual void Sleep()
        {
            if (!StayOn)
            {
                Animator.SetTrigger("TurnOff");
            }
        }

        public void SetColor(Color color)
        {
            _light.SetColor(color);
        }

        public void Activate(IActivator activator = default)
        {
            _broken = true;
            //Hero.Instance.Jumper.Bounce(Transform.position);
            Sleep();
        }

        public override void DoReset()
        {
            base.DoReset();
            
            _broken = false;
            Wake();
        }

        public void Deactivate(IActivator activator = default) { }
    }
}