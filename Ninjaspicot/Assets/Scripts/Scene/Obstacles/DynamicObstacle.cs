using System.Collections;
using UnityEngine;

public class DynamicObstacle : Obstacle, IDynamic
{
    [SerializeField] private PoolableType _poolableType;
    [SerializeField] protected float _customSpeed;

    private Rigidbody2D _rigidbody;
    public Rigidbody2D Rigidbody { get { if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody2D>() ?? GetComponentInChildren<Rigidbody2D>() ?? GetComponentInParent<Rigidbody2D>(); return _rigidbody; } }
    public bool DynamicActive { get; protected set; }

    public PoolableType PoolableType => _poolableType;

    protected override void Awake()
    {
        base.Awake();
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

    public override void Activate()
    {
        DynamicActive = true;
        base.Activate();
    }

    public override void Deactivate()
    {
        DynamicActive = false;
        base.Deactivate();
    }
}
