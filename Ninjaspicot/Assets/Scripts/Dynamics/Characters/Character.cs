using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ZepLink.RiceNinja.Dynamics.Abstract;
using ZepLink.RiceNinja.Dynamics.Effects.Lights;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Manageables.Audios;
using ZepLink.RiceNinja.ServiceLocator.Services;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Characters
{
    public abstract class Character : Dynamic, IRaycastable, IKillable
    {
        [SerializeField] protected CustomColor _lightColor;
        [SerializeField] protected bool _startAwake;

        protected SpriteRenderer _renderer;
        public virtual SpriteRenderer Renderer
        {
            get
            {
                if (BaseUtils.IsNull(_renderer))
                {
                    _renderer = GetComponent<SpriteRenderer>();
                    if (BaseUtils.IsNull(_renderer))
                    {
                        _renderer = GetComponentInChildren<SpriteRenderer>();
                    }
                }

                return _renderer;
            }
        }

        private Collider2D _collider;
        public Collider2D Collider
        {
            get
            {
                if (BaseUtils.IsNull(_collider))
                {
                    _collider = GetComponent<Collider2D>();
                    if (BaseUtils.IsNull(_collider))
                    {
                        _collider = GetComponentInChildren<Collider2D>();
                    }
                }

                return _collider;
            }
        }

        public Image Image { get; private set; }

        public bool Dead { get; set; }

        protected CharacterLight _characterLight;
        protected AudioSource _audioSource;
        protected IPoolService _poolService;
        protected ICameraService _cameraService;
        protected IAudioService _audioService;

        protected virtual void Awake()
        {
            _audioService = ServiceFinder.Get<IAudioService>();
            _poolService = ServiceFinder.Get<IPoolService>();
            _cameraService = ServiceFinder.Get<ICameraService>();
            _audioSource = GetComponent<AudioSource>();
            _characterLight = GetComponentInChildren<CharacterLight>();
            Image = GetComponent<Image>();
        }

        protected virtual void Start()
        {
            _characterLight?.SetColor(_lightColor);
        }

        public abstract void Die(Transform killer = null, AudioFile sound = null, float volume = 1f);
        public abstract IEnumerator Dying();
    }
}