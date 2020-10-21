using UnityEngine;

public class DynamicNinjaCollider : MonoBehaviour, IPoolable
{
    [SerializeField] private PoolableType _poolableType;
    public PoolableType PoolableType => _poolableType;
    public bool Active { get; private set; }
    public Transform Transform { get; private set; }

    private void Awake()
    {
        Transform = transform;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        Active = true;
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        Active = false;
    }

    public void Pool(Vector3 position, Quaternion rotation, float size)
    {
        transform.position = new Vector3(position.x, position.y, -5);
        transform.rotation = rotation;
    }
}
