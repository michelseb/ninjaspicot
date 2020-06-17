using UnityEngine;

public class CompositeContainer : MonoBehaviour, IPoolable, IRaycastable
{
    private int _id;
    public PoolableType PoolableType => PoolableType.None;
    public int Id { get { if (_id == 0) _id = gameObject.GetInstanceID(); return _id; } }

    private ScenesManager _scenesManager;

    private void Awake()
    {
        _scenesManager = ScenesManager.Instance;
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void Pool(Vector3 position, Quaternion rotation)
    {
        _scenesManager.MoveObjectToCurrentScene(gameObject);
        gameObject.SetActive(true);
    }
}
