using System.Collections;
using UnityEngine;

public class DynamicCollider : MonoBehaviour, IPoolable
{
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void Pool(Vector3 position, Quaternion rotation)
    {
        gameObject.SetActive(true);
        transform.position = new Vector3(position.x, position.y, -5);
        transform.rotation = rotation;
    }
}
