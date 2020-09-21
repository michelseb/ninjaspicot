using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class Ninja : MonoBehaviour, IKillable, IRaycastable
{
    [SerializeField] private CustomColor _lightColor;
    private int _id;
    public int Id { get { if (_id == 0) _id = gameObject.GetInstanceID(); return _id; } }
    public bool Dead { get; set; }
    public Jumper Jumper { get; private set; }
    public Stickiness Stickiness { get; private set; }
    public SpriteRenderer Renderer { get; private set; }
    public Image Image { get; private set; }

    private CharacterLight _characterLight;
    protected CameraBehaviour _cameraBehaviour;
    protected PoolManager _poolManager;
    protected AudioSource _audioSource;
    protected AudioManager _audioManager;
    public Transform Transform { get; private set; }

    protected virtual void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioManager = AudioManager.Instance;
        Jumper = GetComponent<Jumper>() ?? GetComponentInChildren<Jumper>();
        Stickiness = GetComponent<Stickiness>() ?? GetComponentInChildren<Stickiness>();
        _cameraBehaviour = CameraBehaviour.Instance;
        Renderer = GetComponent<SpriteRenderer>();
        Image = GetComponent<Image>();
        Transform = transform;
    }

    protected virtual void Start()
    {
        _poolManager = PoolManager.Instance;
        _characterLight = _poolManager.GetPoolable<CharacterLight>(transform.position, Quaternion.identity);
        _characterLight.SetColor(ColorUtils.GetColor(_lightColor));
    }

    protected virtual void Update()
    {
        _characterLight.transform.position = transform.position;
    }

    public virtual void Die(Transform killer)
    {
        if (Dead)
            return;

        Dead = true;
        Stickiness.Detach();
        Jumper.CancelJump();
        SetAllBehavioursActivation(false, false);

        //if (killer != null)
        //{
        //    Stickiness.Rigidbody.AddForce((killer.position - transform.position) * 10000, ForceMode2D.Impulse);
        //    Stickiness.Rigidbody.AddTorque(20000, ForceMode2D.Impulse);
        //}

        StartCoroutine(Dying());
    }

    public virtual IEnumerator Dying()
    {
        yield return null;
    }

    public void SetJumpingActivation(bool active)
    {
        Jumper.Active = active;
    }

    public void SetStickinessActivation(bool active)
    {
        Stickiness.Active = active;
    }

    public void SetWalkingActivation(bool active, bool grounded)
    {
        if (!active)
        {
            Stickiness.StopWalking(grounded);
        }

        Stickiness.CanWalk = active;
    }

    public virtual void SetAllBehavioursActivation(bool active, bool grounded)
    {
        SetJumpingActivation(active);
        SetStickinessActivation(active);
        SetWalkingActivation(active, grounded);
    }

    public virtual bool NeedsToWalk()
    {
        return false;
    }
}
