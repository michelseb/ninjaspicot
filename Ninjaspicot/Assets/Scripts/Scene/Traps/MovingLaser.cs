using UnityEngine;

public class MovingLaser : Laser
{
    [SerializeField] protected float _distance;
    [SerializeField] protected float _speed;

    private float _initialYPos;
    private int _direction;

    protected override void Start()
    {
        base.Start();
        _initialYPos = Transform.position.y;
        _direction = 1;
    }

    protected override void Update()
    {
        if (!_active)
            return;

        if (Mathf.Sign(_direction) * (Transform.position.y - _initialYPos) >= _distance)
        {
            _direction = -_direction;
        }

        Transform.Translate(0, _speed * Mathf.Sign(_direction) * Time.deltaTime, 0);
        SetPointsPosition();
    }

    protected override void SetPointsPosition()
    {
        base.SetPointsPosition();

        _laser.SetPosition(0, _start.position + _start.right);
        _laser.SetPosition(_pointsAmount - 1, _end.position + _end.right);
    }
}
