using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Interfaces;
using ZepLink.RiceNinja.Manageables.Audios;
using ZepLink.RiceNinja.ServiceLocator.Services;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Utilities.Interactives
{
    public class Door : Dynamic, IActivable, IResettable
    {
        [SerializeField] private bool _startOpened;

        private bool _opened;
        private Animator _animator;
        private ParticleSystem _dust;
        private AudioSource _audioSource;
        private IAudioService _audioService;
        private AudioFile _bang;
        private AudioFile _open;

        private void Awake()
        {
            _audioService = ServiceFinder.Get<IAudioService>();
            _animator = GetComponent<Animator>();
            _dust = GetComponentInChildren<ParticleSystem>();
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            _bang = _audioService.FindAudioByName("DoorBang");
            _open = _audioService.FindAudioByName("DoorOpen");
            if (_startOpened)
            {
                Activate(null);
            }
        }

        public void Activate(IActivator activator = default)
        {
            if (_opened)
                return;

            _opened = true;
            _animator.SetTrigger("Open");

            //foreach (var door in _doors)
            //{
            //    door.DetachHero();
            //}
        }

        public void Deactivate(IActivator activator = default)
        {
            if (!_opened)
                return;

            _opened = false;
            _animator.SetTrigger("Close");
        }

        public void Open()
        {
            _audioService.PlaySound(_audioSource, _open, .3f);
        }

        public void Dust()
        {
            _dust.Play();
            _audioService.PlaySound(_audioSource, _bang, .5f);
        }

        public void DoReset()
        {
            if (!_startOpened)
            {
                _opened = true;
                Deactivate(null);
            }
        }
    }
}