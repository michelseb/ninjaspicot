using System.Collections;
using UnityEngine;

public class Ninja : MonoBehaviour, IDynamic, IKillable
{
    private Rigidbody2D _rigidbody;
    public bool Dead { get; set; }
    public JumpManager JumpManager { get; private set; }
    public Stickiness Stickiness { get; private set; }
    public SpriteRenderer Renderer { get; private set; }
    public Rigidbody2D Rigidbody { get { if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody2D>(); return _rigidbody; } }

    protected CameraBehaviour _cameraBehaviour;


    protected virtual void Awake()
    {
        JumpManager = GetComponent<JumpManager>() ?? GetComponentInChildren<JumpManager>();
        Stickiness = GetComponent<Stickiness>() ?? GetComponentInChildren<Stickiness>();
        _cameraBehaviour = CameraBehaviour.Instance;
        Renderer = GetComponent<SpriteRenderer>();
    }


    public virtual void Die(Transform killer)
    {
        if (Dead)
            return;

        Dead = true;
        Stickiness.Detach();
        SetAllBehavioursActivation(false);

        Renderer.color = Color.red;

        if (killer != null)
        {
            var killerRenderer = killer.GetComponent<SpriteRenderer>();
            if (killerRenderer != null)
            {
                killerRenderer.color = Color.red;
            }
            Rigidbody.AddForce(killer.position - transform.position, ForceMode2D.Impulse);
            Rigidbody.AddTorque(20, ForceMode2D.Impulse);
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

    public void SetAllBehavioursActivation(bool active)
    {
        SetJumpingActivation(active);
        SetStickinessActivation(active);
        SetWalkingActivation(active);
    }
}
