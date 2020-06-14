using UnityEngine;

public class CompositeContainer : MonoBehaviour, IPoolable
{
    public PoolableType PoolableType => PoolableType.None;
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
