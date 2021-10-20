using UnityEngine;

public class AimableLocation : MonoBehaviour, IFocusable, ISceneryWakeable
{
    private Hero _hero;
    public Hero Hero { get { if (_hero == null) _hero = Hero.Instance; return _hero; } }
    public bool IsSilent => true;

    private Transform _transform;
    public Transform Transform { get { if (_transform == null) _transform = transform; return _transform; } }

    public bool Active { get; set; }
    public bool Taken => !Active;

    private Zone _zone;
    public Zone Zone { get { if (Utils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

    protected Animator _animator;
    protected SpriteRenderer _renderer;

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void Sleep()
    {
        _renderer.enabled = false;
        Active = false;
    }

    public void Wake()
    {
        _renderer.enabled = true;
        Active = true;
    }
}
