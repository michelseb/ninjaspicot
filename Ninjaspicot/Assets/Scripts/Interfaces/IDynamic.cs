using UnityEngine;

public interface IDynamic
{
    bool DynamicActive { get; }
    Rigidbody2D Rigidbody { get; }
}
