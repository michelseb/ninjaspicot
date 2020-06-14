﻿using UnityEngine;

public enum PoolableType
{
    None,
    EnemyNinja,
    Bullet,
    CloudCollider,
    RockyPlatformCollider,
    HeroCollider,
    DirectionRockCollider
}

public interface IPoolable
{
    PoolableType PoolableType { get; }
    void Pool(Vector3 position, Quaternion rotation);
    void Deactivate();
}
