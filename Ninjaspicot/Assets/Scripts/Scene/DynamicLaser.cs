using UnityEngine;

public class DynamicLaser : Laser
{
    [SerializeField] private float _toggleTime;

    private bool _isToggled;
    private float _remainingTime;

    protected override void Start()
    {
        base.Start();
        _remainingTime = _toggleTime;
    }

    protected override void Update()
    {
        base.Update();

        if (!_active)
            return;

        _remainingTime -= Time.deltaTime;
        if (_remainingTime < 0)
        {
            Toggle(ref _isToggled);
            _remainingTime = _toggleTime;
        }
    }

    protected override void InitPointsPosition()
    {
        _laser.SetPosition(0, _start.position);
        _laser.SetPosition(_pointsAmount - 1, _end.position);
    }

    private void Toggle(ref bool toggled)
    {
        toggled = !toggled;
        _collider.enabled = toggled;
        _laser.enabled = toggled;
    }
}
