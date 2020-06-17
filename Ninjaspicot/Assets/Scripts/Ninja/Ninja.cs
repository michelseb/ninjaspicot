using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class Ninja : MonoBehaviour, IKillable, IRaycastable
{
    private int _id;
    public int Id { get { if (_id == 0) _id = gameObject.GetInstanceID(); return _id; } }
    public bool Dead { get; set; }
    public Jumper JumpManager { get; private set; }
    public Stickiness Stickiness { get; private set; }
    public SpriteRenderer Renderer { get; private set; }
    public Image Image { get; private set; } 

    protected CameraBehaviour _cameraBehaviour;

    protected virtual void Awake()
    {
        JumpManager = GetComponent<Jumper>() ?? GetComponentInChildren<Jumper>();
        Stickiness = GetComponent<Stickiness>() ?? GetComponentInChildren<Stickiness>();
        _cameraBehaviour = CameraBehaviour.Instance;
        Renderer = GetComponent<SpriteRenderer>();
        Image = GetComponent<Image>();
    }


    public virtual void Die(Transform killer)
    {
        if (Dead)
            return;

        Dead = true;
        Stickiness.Detach();
        JumpManager.ReinitJump();
        SetAllBehavioursActivation(false);

        if (killer != null)
        {
            Stickiness.Rigidbody.AddForce(killer.position - transform.position, ForceMode2D.Impulse);
            Stickiness.Rigidbody.AddTorque(20, ForceMode2D.Impulse);
        }

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

    public void SetWalkingActivation(bool active)
    {
        if (!active)
        {
            Stickiness.StopWalking();
        }

        Stickiness.CanWalk = active;
    }

    public virtual void SetAllBehavioursActivation(bool active)
    {
        SetJumpingActivation(active);
        SetStickinessActivation(active);
        SetWalkingActivation(active);
    }

    public virtual bool NeedsToWalk()
    {
        return false;
    }
}
