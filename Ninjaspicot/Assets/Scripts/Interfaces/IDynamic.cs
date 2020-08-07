using UnityEngine;

public interface IDynamic
{
    PoolableType PoolableType { get; }
    Transform Transform { get; }
    bool DynamicActive { get; }
    Rigidbody2D Rigidbody { get; }

    //Deactivates dynamic temporarily
    void LaunchQuickDeactivate();
}
