using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class Character : MonoBehaviour, IRaycastable, IKillable
{
    [SerializeField] protected CustomColor _lightColor;
    private int _id;
    public int Id { get { if (_id == 0) _id = gameObject.GetInstanceID(); return _id; } }
    private SpriteRenderer _renderer;
    public SpriteRenderer Renderer
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

    public Image Image { get; private set; }

    private Transform _transform;
    public Transform Transform { get { if (Utils.IsNull(_transform)) _transform = transform; return _transform; } }

    public bool Dead { get; set; }

    protected CharacterLight _characterLight;
    protected CameraBehaviour _cameraBehaviour;
    protected PoolManager _poolManager;
    protected AudioSource _audioSource;
    protected AudioManager _audioManager;

    protected virtual void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioManager = AudioManager.Instance;
        _characterLight = GetComponentInChildren<CharacterLight>();
        _cameraBehaviour = CameraBehaviour.Instance;
        Image = GetComponent<Image>();
    }

    protected virtual void Start()
    {
        _poolManager = PoolManager.Instance;
        _characterLight?.SetColor(ColorUtils.GetColor(_lightColor));
    }

    public abstract void Die(Transform killer = null, Audio sound = null, float volume = 1f);
    public abstract IEnumerator Dying();
}
