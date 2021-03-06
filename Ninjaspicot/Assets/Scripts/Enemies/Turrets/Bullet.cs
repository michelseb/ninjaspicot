﻿using UnityEngine;

public class Bullet : MonoBehaviour, IPoolable
{
    public float Speed { get; set; }

    public PoolableType PoolableType => PoolableType.Bullet;

    private float _currentLifeTime;
    private const float LIFE_TIME = 5;
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    private void Start()
    {
        _currentLifeTime = LIFE_TIME;
    }

    private void Update()
    {
        _transform.Translate(0, Speed * Time.deltaTime, 0);
        _currentLifeTime -= Time.deltaTime;
        if (_currentLifeTime <= 0)
        {
            Deactivate();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") || collision.CompareTag("DynamicWall") || collision.CompareTag("TutorialWall"))
        {
            Deactivate();
        }
        if (collision.CompareTag("hero"))
        {
            collision.GetComponent<Hero>().Die(_transform);
            Deactivate();
        }

    }

    public void Pool(Vector3 position, Quaternion rotation)
    {
        _transform.position = new Vector3(position.x, position.y, -5);
        _transform.rotation = rotation;
        _currentLifeTime = LIFE_TIME;
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }
}
