using System.Collections;
using UnityEngine;

public class DynamicObstacle : Obstacle, IDynamic, IActivable
{
    [SerializeField] private PoolableType _poolableType;
    [SerializeField] protected float _speed;

    private Rigidbody2D _rigidbody;
    public Rigidbody2D Rigidbody { get { if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody2D>() ?? GetComponentInChildren<Rigidbody2D>() ?? GetComponentInParent<Rigidbody2D>(); return _rigidbody; } }
    public bool DynamicActive { get; protected set; }

    public PoolableType PoolableType => _poolableType;

    public virtual void Awake()
    {
        Activate();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (!DynamicActive || !collision.collider.CompareTag("hero"))
            return;

        var stickiness = collision.collider.GetComponent<Stickiness>();

        if (stickiness == null)
            return;

        stickiness.SetContactPosition(collision.contacts[collision.contacts.Length - 1].point);

        var dynamicInteraction = collision.collider.GetComponent<DynamicInteraction>();

        if (!dynamicInteraction.Active || dynamicInteraction.Interacting)
            return;

        if (GetComponent<EnemyNinja>() != null)
            return;

        dynamicInteraction.StartInteraction(this);
    }

    public void LaunchQuickDeactivate()
    {
        StartCoroutine(QuickDeactivate());
    }

    private IEnumerator QuickDeactivate()
    {
        Deactivate();
        yield return new WaitForSeconds(.5f);
        Activate();
    }

    public void Activate()
    {
        DynamicActive = true;
    }

    public void Deactivate()
    {
        DynamicActive = false;
    }
}
