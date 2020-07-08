using UnityEngine;

public class FallingPlatform : DynamicObstacle
{
    private float _currentSpeed;
    private float _remainingTime;
    private Vector3 _initialPosition;
    private bool _active;

    private const float FALL_TIME = 10f;
    private const float MAX_SPEED = 20f;
    private const float DEFAULT_SPEED = 1f;

    protected virtual void Start()
    {
        _initialPosition = transform.position;
        InitPlatform();
        _customSpeed = _customSpeed > 0 ? _customSpeed : DEFAULT_SPEED;
    }

    private void Update()
    {
        if (!_active)
            return;

        _currentSpeed += _customSpeed * Time.deltaTime;
        _currentSpeed = Mathf.Clamp(_currentSpeed, 0, MAX_SPEED);

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
        _remainingTime = FALL_TIME;
        transform.position = _initialPosition;
    }
}
