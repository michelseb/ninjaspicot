using System.Collections;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Abstract;
using ZepLink.RiceNinja.Dynamics.Characters.Ninjas.MainCharacter;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Dynamics.Scenery.Zones;
using ZepLink.RiceNinja.Interfaces;
using ZepLink.RiceNinja.Manageables.Audios;
using ZepLink.RiceNinja.ServiceLocator.Services;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Bonuses
{
    public abstract class Bonus : Dynamic, IActivable, ISceneryWakeable, IRaycastable, IAudio
    {
        [SerializeField] protected bool _respawn;
        [SerializeField] protected float _respawnTime;

        protected Collider2D[] _colliders;
        protected SpriteRenderer[] _renderers;
        protected Animator _animator;
        private AudioSource _audioSource;
        public AudioSource AudioSource { get { if (BaseUtils.IsNull(_audioSource)) _audioSource = GetComponent<AudioSource>(); return _audioSource; } }

        protected IAudioService _audioService;
        protected Coroutine _temporaryDeactivate;
        protected bool _active;
        protected bool _taken;

        private Zone _zone;
        public Zone Zone { get { if (BaseUtils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }


        protected virtual void Awake()
        {
            _colliders = GetComponentsInChildren<Collider2D>();
            _renderers = GetComponentsInChildren<SpriteRenderer>();
            _animator = GetComponent<Animator>();
            _audioService = ServiceFinder.Get<IAudioService>();
        }

        protected virtual void Start()
        {
            _active = true;
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.TryGetComponent(out IPicker picker) || !picker.CanTake)
                return;

            TakeBy(picker);
        }

        protected IEnumerator TemporaryDeactivation(float time)
        {
            Deactivate();
            yield return new WaitForSeconds(time);
            Activate();

            _temporaryDeactivate = null;
        }

        public void Activate(IActivator activator = default)
        {
            if (_taken)
                return;

            _active = true;
            _animator.enabled = true;

            foreach (var collider in _colliders)
            {
                collider.enabled = true;
            }

            foreach (var renderer in _renderers)
            {
                renderer.enabled = true;
            }
        }

        public void Deactivate(IActivator activator = default)
        {
            _active = false;
            _animator.enabled = false;

            foreach (var collider in _colliders)
            {
                collider.enabled = false;
            }

            foreach (var renderer in _renderers)
            {
                renderer.enabled = false;
            }
        }

        public void Sleep()
        {
            _animator.SetTrigger("Sleep");
        }

        public void Wake()
        {
            _animator.SetTrigger("Wake");
        }

        public virtual void TakeBy(IPicker picker)
        {
            if (_respawn)
            {
                if (_active)
                {
                    _temporaryDeactivate = StartCoroutine(TemporaryDeactivation(_respawnTime));
                }
            }
            else
            {
                _taken = true;
                Deactivate();
            }
        }
    }
}