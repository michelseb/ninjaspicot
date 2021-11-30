using System.Collections;
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
        _laser.SetPosition(0, _startPosition);
        _laser.SetPosition(_pointsAmount - 1, _endPosition);
        SetPointsPosition();
    }

    private void Toggle(ref bool turnedOn)
    {
        turnedOn = !turnedOn;

        if (turnedOn)
        {
            StartCoroutine(TurnOn());
        }
        else
        {
            StartCoroutine(TurnOff());
        }
    }

    private IEnumerator TurnOff()
    {
        _collider.enabled = false;
        yield return new WaitForSecondsRealtime(.1f);
        _activationIndicator.ForEach(x => x.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear));
        _laser.enabled = false;
    }

    private IEnumerator TurnOn()
    {
        _activationIndicator.ForEach(x => x.Play());
        yield return new WaitForSecondsRealtime(.6f);
        _laser.enabled = true;
        _collider.enabled = true;
    }
}
