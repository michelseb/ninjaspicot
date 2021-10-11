using System.Collections;
using UnityEngine;

public abstract class Bonus : MonoBehaviour, IActivable, ISceneryWakeable, IRaycastable, IResettable
{
    [SerializeField] protected bool _respawn;
    [SerializeField] protected float _respawnTime;

    protected Collider2D[] _colliders;
    protected SpriteRenderer[] _renderers;
    protected Animator _animator;
    protected AudioSource _audioSource;
    protected AudioManager _audioManager;
    protected Coroutine _temporaryDeactivate;
    protected Hero _hero;
    protected bool _active;
    protected bool _taken;

    private Zone _zone;
    public Zone Zone { get { if (Utils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

    private int _id;
    public int Id { get { if (_id == 0) _id = gameObject.GetInstanceID(); return _id; } }

    protected virtual void Awake()
    {
        _colliders = GetComponentsInChildren<Collider2D>();
        _renderers = GetComponentsInChildren<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _audioManager = AudioManager.Instance;
    }

    protected virtual void Start()
    {
        _active = true;
        _hero = Hero.Instance;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("hero") || Hero.Instance.Dead)
            return;

        Take();
    }

    protected IEnumerator TemporaryDeactivation(float time)
    {
        Deactivate();
        yield return new WaitForSeconds(time);
        Activate();

        _temporaryDeactivate = null;
    }

    public void Activate()
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

    public void Deactivate()
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

    public virtual void Take()
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

    public virtual void DoReset()
    {
        _animator.Rebind();
        _animator.Update(0);
        //_animator.Play(_animator.GetCurrentAnimatorStateInfo(0).fullPathHash);
    }
}
