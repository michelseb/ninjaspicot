using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class Ninja : MonoBehaviour, INinja, IKillable, IRaycastable
{
    private int _id;
    public int Id { get { if (_id == 0) _id = gameObject.GetInstanceID(); return _id; } }
    public bool Dead { get; set; }
    public Jumper JumpManager { get; private set; }
    public Stickiness Stickiness { get; private set; }
    public SpriteRenderer Renderer { get; private set; }
    public Image Image { get; private set; }
    public bool ReadyToWalk => NeedsToWalk();


    protected CameraBehaviour _cameraBehaviour;
    protected PoolManager _poolManager;
    protected Transform _transform;

    protected virtual void Awake()
    {
        _poolManager = PoolManager.Instance;
        JumpManager = GetComponent<Jumper>() ?? GetComponentInChildren<Jumper>();
        Stickiness = GetComponent<Stickiness>() ?? GetComponentInChildren<Stickiness>();
        _cameraBehaviour = CameraBehaviour.Instance;
        Renderer = GetComponent<SpriteRenderer>();
        Image = GetComponent<Image>();
        _transform = transform;
    }


    public virtual void Die(Transform killer)
    {
        if (Dead)
            return;

        Dead = true;
        Stickiness.Detach();
        JumpManager.ReinitJump();
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
        JumpManager.Active = active;
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
