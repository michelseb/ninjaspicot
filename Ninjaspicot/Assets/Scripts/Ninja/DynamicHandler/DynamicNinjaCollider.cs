﻿using UnityEngine;

public class DynamicNinjaCollider : MonoBehaviour, IPoolable
{
    [SerializeField] private PoolableType _poolableType;
    public PoolableType PoolableType => _poolableType;
    public bool Active { get; private set; }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        Active = false;
    }

    public void Pool(Vector3 position, Quaternion rotation)
    {
        gameObject.SetActive(true);
        Active = true;
        transform.position = new Vector3(position.x, position.y, -5);
        transform.rotation = rotation;
    }
}
