using UnityEngine;

public interface IPoolable
{
    void Pool(Vector3 position, Quaternion rotation);
    void Deactivate();
}
