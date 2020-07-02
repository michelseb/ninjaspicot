using System.Collections;
using UnityEngine;

public class LaserAim : Aim
{
    private int _currentAngle;
    private Coroutine _laserize;
    private Coroutine _unlaserize;

    public bool Charged { get; private set; }

    private const int LASER_WIDTH = 5;

    protected override void Awake()
    {
        base.Awake();
        _currentAngle = _viewAngle;
    }

    protected override void Update()
    {
        base.Update();
        if (TargetInRange)
        {
            TargetInRange = (Hero.Instance.transform.position - transform.position).sqrMagnitude < _size * _size;
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("hero"))
            return;

        if (Charged)
        {
            Hero.Instance.Die(transform);
        }
    }

    protected override void OnTriggerExit2D(Collider2D collision) { }

    public void StartLaserize()
    {
        if (_unlaserize != null)
        {
            StopCoroutine(_unlaserize);
        }

        _laserize = StartCoroutine(Laserize());
    }

    public void StopLaserize()
    {
        if (_laserize != null)
        {
            StopCoroutine(_laserize);
        }
        Charged = false;
        _unlaserize = StartCoroutine(Unlaserize());
    }

    private IEnumerator Laserize()
    {
        while(_currentAngle > LASER_WIDTH)
        {
            _currentAngle--;
            yield return null;
        }

        Charged = true;
        _laserize = null;
    }

    private IEnumerator Unlaserize()
    {
        while (_currentAngle < _viewAngle)
        {
            _currentAngle++;
            yield return null;
        }

        _unlaserize = null;
    }
}
