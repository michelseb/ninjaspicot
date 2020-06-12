using System.Collections;
using UnityEngine;

public class EnemyNinja : Ninja, IRaycastable, IPoolable
{
    [SerializeField] private bool _patrolling;
    [SerializeField] private Dir _direction;

    protected virtual void Start()
    {
        Stickiness.NinjaDir = _direction;

        if (_patrolling)
        {
            Stickiness.StartWalking();
        }
    } 

    public void Pool(Vector3 position, Quaternion rotation)
    {
        gameObject.SetActive(true);
        transform.position = position;
        transform.rotation = rotation;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("hero"))
        {
            var hero = collision.collider.GetComponent<Hero>();
            if (hero == null)
                return;

            hero.Die(transform);
        }
    }

    public override IEnumerator Dying()
    {
        Deactivate();
        yield return base.Dying();
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public override bool NeedsToWalk()
    {
        return _patrolling;
    }
}
