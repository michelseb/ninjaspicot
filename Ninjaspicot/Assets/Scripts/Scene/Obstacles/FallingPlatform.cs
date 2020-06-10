using UnityEngine;

public class FallingPlatform : DynamicObstacle
{
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _fallTime;

    private float _currentSpeed;
    private float _remainingTime;
    private Vector3 _initialPosition;
    private bool _active;

    protected virtual void Start()
    {
        _initialPosition = transform.position;
        InitPlatform();
    }

    private void Update()
    {
        if (!_active)
            return;

        _currentSpeed += _speed * Time.deltaTime;
        _currentSpeed = Mathf.Clamp(_currentSpeed, 0, _maxSpeed);

        Rigidbody.MovePosition(Rigidbody.position + new Vector2(0, -_currentSpeed));

        _remainingTime -= Time.deltaTime;

        if (_remainingTime <= 0)
        {
            InitPlatform();
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if (collision.collider.CompareTag("hero"))
        {
            _active = true;
        }
    }

    private void InitPlatform()
    {
        _active = false;
        _currentSpeed = 0;
        _remainingTime = _fallTime;
        transform.position = _initialPosition;
    }
}
