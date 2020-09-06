using UnityEngine;

public enum PoolableType
{
    None,
    EnemyNinja,
    Marauder,
    Bullet,
    Touch1,
    Touch2,
    Cloud,
    Platform,
    Shelf,
    Rice,
    IndicationRock,
    Plate,
    Shovel
}

public interface IPoolable : IActivable
{
    PoolableType PoolableType { get; }
    void Pool(Vector3 position, Quaternion rotation, float size = 1);
}
