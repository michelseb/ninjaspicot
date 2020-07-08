using UnityEngine;

public class Poppable : DynamicObstacle 
{
    [SerializeField] private float _maxSpeed;
    [SerializeField] private Transform _bottom;

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

        _currentSpeed += _customSpeed * Time.deltaTime;
        _currentSpeed = Mathf.Clamp(_currentSpeed, 0, _maxSpeed);

        Rigidbody.MovePosition(Rigidbody.position + new Vector2(0, -_currentSpeed));

        var hit = Utils.RayCast(_bottom.position, Vector2.down, .1f, Id);
        if (hit && !hit.collider.CompareTag("hero"))
        {
            _active = false;
            DynamicActive = false;
        }
    }
}
