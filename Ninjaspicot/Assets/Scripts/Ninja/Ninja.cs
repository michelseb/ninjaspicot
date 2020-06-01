using System.Collections;
using UnityEngine;

public class Ninja : DynamicEntity, IDestructable
{
    public StealthyMovement Movement { get; private set; }
    public Stickiness Stickiness { get; private set; }
    protected CameraBehaviour _cameraBehaviour;


    protected virtual void Awake()
    {
        //SelectDeplacementMode();
        Movement = GetComponent<StealthyMovement>() ?? GetComponentInChildren<StealthyMovement>();
        Stickiness = GetComponent<Stickiness>() ?? GetComponentInChildren<Stickiness>();
        _cameraBehaviour = CameraBehaviour.Instance;
    }


    public virtual void Die(Transform killer)
    {
        SetMovementActivation(false);

        var joint = GetComponent<Joint2D>();
        if (joint != null)
        {
            Destroy(joint);
        }

        if (GetComponent<SpriteRenderer>() != null)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
        }

        if (killer != null)
        {
            if (killer.GetComponent<SpriteRenderer>() != null)
            {
                killer.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
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
        Movement.enabled = active;
    }
}
