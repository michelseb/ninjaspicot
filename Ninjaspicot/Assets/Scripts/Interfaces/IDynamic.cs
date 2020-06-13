using UnityEngine;

public interface IDynamic
{
    PoolableType PoolableType { get; }
    bool DynamicActive { get; }
    Rigidbody2D Rigidbody { get; }
}
