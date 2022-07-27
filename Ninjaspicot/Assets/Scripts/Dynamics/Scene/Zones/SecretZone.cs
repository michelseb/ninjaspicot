using UnityEngine;
using ZepLink.RiceNinja.Manageables.Audios;
using ZepLink.RiceNinja.ServiceLocator.Services;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Zones
{
    public class SecretZone : Zone
    {
        private AudioSource _audioSource;
        private IAudioService _audioService;
        private AudioFile _discoverSound;
        private bool _discovered;
        private bool _opened;

        protected override void Awake()
        {
            base.Awake();
            _audioSource = GetComponent<AudioSource>();
            _audioService = ServiceFinder.Get<IAudioService>();
        }

        protected override void Start()
        {
            base.Start();
            _discoverSound = _audioService.FindAudioByName("Discover");
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("hero"))
                return;

            OnTriggerStay2D(collision);
        }

        protected virtual void OnTriggerStay2D(Collider2D collision)
        {
            if (_opened || !collision.CompareTag("hero"))
                return;

            if (!_discovered)
            {
                _audioService.PlaySound(_audioSource, _discoverSound, .3f);
                _discovered = true;
            }

            _zoneService.OpenExtraZone(Id);
            _opened = true;
        }

        protected virtual void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.CompareTag("hero"))
                return;

            Close();
        }

        public override void Close()
        {
            base.Close();

            _opened = false;
            _zoneService.UpdateCurrentZoneCameraBehavior();
        }

        protected override void SetSpawn() { }
    }
}