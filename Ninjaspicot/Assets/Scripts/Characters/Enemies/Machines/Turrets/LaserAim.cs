using System.Collections;
using UnityEngine;

public class LaserAim : Aim
{

    private float _initAngle;
    private Color _initColor;
    private Coroutine _laserize;
    private Coroutine _unlaserize;

    public bool Charged { get; private set; }

    private const int LASER_WIDTH = 2;
    private const float LASERIZE_DURATION = .4f;
    private const float UNLASERIZE_DURATION = .2f;

    protected override void Awake()
    {
        base.Awake();
        _initAngle = _viewAngle;
        //_initSize = _size;
        _initColor = _color;
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("hero"))
            return;

        base.OnTriggerStay2D(collision);
        //if (Charged && TargetAimedAt(collision.transform, Turret.Id) && TargetCentered(_transform, CurrentTarget, Turret.Id))
        //{
        //    Hero.Instance.Die(_transform);
        //}
    }

    public void StartLaserize()
    {
        if (_laserize != null)
            return;

        if (_unlaserize != null)
        {
            StopCoroutine(_unlaserize);
        }

        _laserize = StartCoroutine(Laserize(LASERIZE_DURATION));
    }

    public void StopLaserize()
    {
        if (_unlaserize != null)
            return;

        if (_laserize != null)
        {
            StopCoroutine(_laserize);
        }
        Charged = false;
        _unlaserize = StartCoroutine(Unlaserize(UNLASERIZE_DURATION));
    }

    private IEnumerator Laserize(float duration)
    {
        float elapsedTime = 0f;
        var currentAngle = _viewAngle;
        //var currentSize = _size;
        var currentColor = _color;
        //var targetSize = _size * GROWTH_FACTOR;
        var targetColor = ColorUtils.GetColor(CustomColor.Red, TRANSPARENCY);

        while (elapsedTime < duration)
        {
            _viewAngle = Mathf.Lerp(currentAngle, LASER_WIDTH, elapsedTime / duration);
            //_size = Mathf.Lerp(currentSize, targetSize, elapsedTime / duration);
            _color = Color.Lerp(currentColor, targetColor, elapsedTime / duration);
            SetColor(_color);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Charged = true;
        _laserize = null;
    }

    private IEnumerator Unlaserize(float duration)
    {
        float elapsedTime = 0f;
        var currentAngle = _viewAngle;
        //var currentSize = _size;
        var currentColor = _color;

        while (elapsedTime < duration)
        {
            _viewAngle = Mathf.Lerp(currentAngle, _initAngle, elapsedTime / duration);
            //_size = Mathf.Lerp(currentSize, _initSize, elapsedTime / duration);
            _color = Color.Lerp(currentColor, _initColor, elapsedTime / duration);
            SetColor(_color);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _unlaserize = null;
    }
}
