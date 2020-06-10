using UnityEngine;

public class Poppable : DynamicObstacle 
{
    [SerializeField] private float _maxSpeed;

    private float _currentSpeed;
    private bool _active;

    private void OnEnable()
    {
        _active = true;
    }

    private void Update()
    {
        if (!_active)
            return;

        _currentSpeed += _speed * Time.deltaTime;
        _currentSpeed = Mathf.Clamp(_currentSpeed, 0, _maxSpeed);

        Rigidbody.MovePosition(Rigidbody.position + new Vector2(0, -_currentSpeed));
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (!collision.collider.CompareTag("hero"))
        {
            _active = false;
        }
    }
}
