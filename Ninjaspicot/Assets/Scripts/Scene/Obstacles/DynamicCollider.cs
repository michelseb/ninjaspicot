using UnityEngine;

public class DynamicCollider : Obstacle, IPoolable
{
    [SerializeField] private PoolableType _poolableType;
    public PoolableType PoolableType => _poolableType;
    public bool Active { get; private set; }

    public override void Activate()
    {
        Active = true;
        gameObject.SetActive(true);
    }

    public override void Deactivate()
    {
        Active = false;
        gameObject.SetActive(false);
    }

    public void Pool(Vector3 position, Quaternion rotation, float size)
    {
        transform.position = new Vector3(position.x, position.y, -5);
        transform.rotation = rotation;
    }
}
