﻿using UnityEngine;
using UnityEngine.UI;

public class ThrownLaser : Laser, IPoolable
{
    [SerializeField] protected float _lifeTime;
    [SerializeField] protected float _speed;

    private Image _startImage;
    private Image _endImage;
    private float _remainingLifeTime;

    public PoolableType PoolableType => PoolableType.None;

    protected override void Awake()
    {
        base.Awake();
        _startImage = _start.GetComponent<Image>();
        _endImage = _end.GetComponent<Image>();
    }

    protected override void Update()
    {
        if (!_active)
            return;

        _remainingLifeTime -= Time.deltaTime;
        if (_remainingLifeTime <= 0)
        {
            Sleep();
        }

        Transform.Translate(0, _speed * Time.deltaTime, 0);
        SetPointsPosition();
    }

    protected override void SetPointsPosition()
    {
        base.SetPointsPosition();

        _laser.SetPosition(0, _start.position + _start.right);
        _laser.SetPosition(_pointsAmount - 1, _end.position + _end.right);
    }

    public void Pool(Vector3 position, Quaternion rotation, float size = 1)
    {
        Transform.position = position;
        _remainingLifeTime = _lifeTime;
    }

    public override void Sleep()
    {
        base.Sleep();
        _startImage.enabled = false;
        _endImage.enabled = false;
    }

    public override void Wake()
    {
        base.Wake();
        _startImage.enabled = true;
        _endImage.enabled = true;
    }
}
