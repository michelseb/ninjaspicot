using System.Collections;
using UnityEngine;

public class Ninja : MonoBehaviour, IKillable
{
    public bool Dead { get; set; }
    public JumpManager JumpManager { get; private set; }
    public Stickiness Stickiness { get; private set; }
    public DynamicInteraction DynamicInteraction { get; private set; }
    public SpriteRenderer Renderer { get; private set; }

    protected CameraBehaviour _cameraBehaviour;


    protected virtual void Awake()
    {
        JumpManager = GetComponent<JumpManager>() ?? GetComponentInChildren<JumpManager>();
        Stickiness = GetComponent<Stickiness>() ?? GetComponentInChildren<Stickiness>();
        DynamicInteraction = GetComponent<DynamicInteraction>() ?? GetComponentInChildren<DynamicInteraction>();
        _cameraBehaviour = CameraBehaviour.Instance;
        Renderer = GetComponent<SpriteRenderer>();
    }


    public virtual void Die(Transform killer)
    {
        if (Dead)
            return;

        Dead = true;
        DynamicInteraction.StopInteraction(false);
        Stickiness.Detach();
        JumpManager.ReinitJump();
        SetAllBehavioursActivation(false);

        Renderer.color = Color.red;

        if (killer != null)
        {
            var killerRenderer = killer.GetComponent<SpriteRenderer>();
            if (killerRenderer != null)
            {
                killerRenderer.color = Color.red;
            }
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

    public void SetInteractionActivation(bool active)
    {
        DynamicInteraction.Active = active;
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
        SetInteractionActivation(active);
    }
}
