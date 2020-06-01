using UnityEngine;

public abstract class DynamicEntity : MonoBehaviour
{
    protected Rigidbody2D _rigidbody;
    public Rigidbody2D Rigidbody { get { if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody2D>() ?? GetComponentInChildren<Rigidbody2D>() ?? GetComponentInParent<Rigidbody2D>(); return _rigidbody; } }
}
