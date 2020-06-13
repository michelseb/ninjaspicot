using UnityEngine;

public enum PoolableType
{
    EnemyNinja,
    DashEffect,
    Trajectory,
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
