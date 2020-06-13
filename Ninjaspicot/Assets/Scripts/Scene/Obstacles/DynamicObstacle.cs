using UnityEngine;

public class DynamicObstacle : Obstacle, IDynamic
{
    [SerializeField] private PoolableType _poolableType;
    [SerializeField] protected float _speed;

    private Rigidbody2D _rigidbody;
    public Rigidbody2D Rigidbody { get { if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody2D>() ?? GetComponentInChildren<Rigidbody2D>() ?? GetComponentInParent<Rigidbody2D>(); return _rigidbody; } }
    public bool DynamicActive { get; set; }

    public PoolableType PoolableType => _poolableType;

    public override void Awake()
    {
        base.Awake();
        DynamicActive = true;
    }
}
