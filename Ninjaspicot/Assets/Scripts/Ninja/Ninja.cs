using System.Collections;
using UnityEngine;

public class Ninja : DynamicEntity, IKillable
{
    public bool Dead { get; set; }
    public StealthyMovement Movement { get; private set; }
    public Stickiness Stickiness { get; private set; }
    public SpriteRenderer Renderer { get; private set; }

    protected CameraBehaviour _cameraBehaviour;


    protected virtual void Awake()
    {
        //SelectDeplacementMode();
        Movement = GetComponent<StealthyMovement>() ?? GetComponentInChildren<StealthyMovement>();
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
        SetMovementAndStickinessActivation(false);

        Renderer.color = Color.red;

        if (killer != null)
        {
            var killerRenderer = killer.GetComponent<SpriteRenderer>();
            if (killerRenderer != null)
            {
                killerRenderer.color = Color.red;
            }
            _rigidbody.AddForce(killer.position - transform.position, ForceMode2D.Impulse);
            _rigidbody.AddTorque(20, ForceMode2D.Impulse);
        }

        StartCoroutine(Dying());
    }

    public virtual IEnumerator Dying()
    {
        yield return null;
    }

    public void SetMovementActivation(bool active)
    {
        Movement.StopWalking();
        Movement.Active = active;
    }

    public void SetStickinessActivation(bool active)
    {
        Stickiness.Active = active;
    }

    public void SetMovementAndStickinessActivation(bool active)
    {
        SetMovementActivation(active);
        SetStickinessActivation(active);
    }
}
