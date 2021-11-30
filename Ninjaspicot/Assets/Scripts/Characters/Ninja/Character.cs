using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class Character : Dynamic, IKillable
{
    [SerializeField] protected CustomColor _lightColor;
    [SerializeField] protected bool _startAwake;
    protected SpriteRenderer _renderer;
    public virtual SpriteRenderer Renderer
    {
        get
        {
            if (Utils.IsNull(_renderer))
            {
                _renderer = GetComponent<SpriteRenderer>();
                if (Utils.IsNull(_renderer))
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
            if (Utils.IsNull(_collider))
            {
                _collider = GetComponent<Collider2D>();
                if (Utils.IsNull(_collider))
                {
                    _collider = GetComponentInChildren<Collider2D>();
                }
            }

            return _collider;
        }
    }

    private Image _image;
    public virtual Image Image
    {
        get
        {
            if (Utils.IsNull(_image))
            {
                _image = GetComponent<Image>();
                if (Utils.IsNull(_image))
                {
                    _image = GetComponentInChildren<Image>();
                }
            }

            return _image;
        }
    }

    public bool Dead { get; set; }
    public bool IsSilent => true;
    public bool Taken => false;

    protected CharacterLight _characterLight;
    protected CameraBehaviour _cameraBehaviour;
    protected PoolManager _poolManager;
    protected AudioSource _audioSource;
    protected AudioManager _audioManager;

    protected virtual void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioManager = AudioManager.Instance;
        _poolManager = PoolManager.Instance;
        _characterLight = GetComponentInChildren<CharacterLight>();
        _cameraBehaviour = CameraBehaviour.Instance;
    }

    protected virtual void Start()
    {
        _characterLight?.SetColor(ColorUtils.GetColor(_lightColor));
    }

    public abstract void Die(Transform killer = null, Audio sound = null, float volume = 1f);
    public abstract IEnumerator Dying();
}
