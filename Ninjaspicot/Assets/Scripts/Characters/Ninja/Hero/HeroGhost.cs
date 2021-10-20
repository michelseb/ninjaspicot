﻿using UnityEngine;

public class HeroGhost : MonoBehaviour, IPoolable
{
    [SerializeField] private float _disappearingSpeed;
    [SerializeField] private float _initAlpha;
    [SerializeField] private Color _endColor;

    private SpriteRenderer _renderer;
    private Transform _transform;
    public PoolableType PoolableType => PoolableType.None;
    private Vector3 _initSize;

    private void Awake()
    {
        _transform = transform;
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        var color = _renderer.color;

        if (color.a <= .01f)
        {
            Sleep();
        }
        else
        {
            _renderer.color = Color.Lerp(color, _endColor, Time.deltaTime * _disappearingSpeed);
        }
    }


    public void Pool(Vector3 position, Quaternion rotation, float size)
    {
        _transform.position = position;
        _transform.rotation = rotation;
        _initSize = _transform.localScale;
        _transform.localScale = new Vector3(_transform.localScale.x * size, _transform.localScale.y * size);
    }

    public void Sleep()
    {
        gameObject.SetActive(false);
        _transform.localScale = _initSize;
    }

    public void Wake()
    {
        gameObject.SetActive(true);
        var color = Hero.Instance.Renderer.color;
        _renderer.color = new Color(color.r, color.g, color.b, _initAlpha);
    }

    public void DoReset()
    {
        Sleep();
    }
}
