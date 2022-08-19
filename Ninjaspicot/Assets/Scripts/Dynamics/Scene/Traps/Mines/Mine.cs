using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using ZepLink.RiceNinja.Dynamics.Abstract;
using ZepLink.RiceNinja.Dynamics.Characters.Ninjas.MainCharacter;
using ZepLink.RiceNinja.Dynamics.Effects;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Dynamics.Scenery.Zones;
using ZepLink.RiceNinja.Helpers;
using ZepLink.RiceNinja.Interfaces;
using ZepLink.RiceNinja.ServiceLocator.Services;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Traps.Mines
{
    public class Mine : Dynamic, IActivable, ISceneryWakeable
    {
        protected Image _renderer;
        protected Color _initialColor;
        protected AudioSource _audioSource;
        protected IAudioService _audioService;
        protected Light2D _light;

        private Zone _zone;
        public Zone Zone { get { if (BaseUtils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

        protected virtual void Awake()
        {
            _audioService = ServiceFinder.Get<IAudioService>();
            _renderer = GetComponent<Image>();
            _initialColor = _renderer.color;
            _audioSource = GetComponent<AudioSource>();
            _light = GetComponent<Light2D>();
        }

        protected void OnTriggerEnter2D(Collider2D collider)
        {
            if (!collider.CompareTag("hero") || !collider.TryGetComponent(out Hero hero) || hero.Dead)
                return;

            PoolHelper.Pool<Explosion>(transform.position, transform.rotation);
            _audioService.PlaySound(_audioSource, "Explode");
            hero.Die(transform);
        }

        public virtual void Activate(IActivator activator = default)
        {
            _renderer.color = ColorUtils.Blue;
        }

        public virtual void Deactivate(IActivator activator = default)
        {
            _renderer.color = _initialColor;
        }

        public void Sleep()
        {
            _light.enabled = false;
        }

        public void Wake()
        {
            _light.enabled = true;
        }
    }
}