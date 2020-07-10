using UnityEngine;

public interface IDynamic
{
    PoolableType PoolableType { get; }
    bool DynamicActive { get; }
    Rigidbody2D Rigidbody { get; }

    //Deactivates dynamic temporarily
    void LaunchQuickDeactivate();
}
