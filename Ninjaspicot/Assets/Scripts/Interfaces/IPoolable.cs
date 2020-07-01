﻿using UnityEngine;

public enum PoolableType
{
    None,
    EnemyNinja,
    Marauder,
    Bullet,
    Touch1,
    Touch2
}

public interface IPoolable : IActivable
{
    PoolableType PoolableType { get; }
    void Pool(Vector3 position, Quaternion rotation);
}
