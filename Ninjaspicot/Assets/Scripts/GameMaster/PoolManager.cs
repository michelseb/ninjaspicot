using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _poolableModels;
    [SerializeField] private Transform _poolableParent;

    private List<IPoolable> _poolables;

    private static PoolManager _instance;
    public static PoolManager Instance { get { if (_instance == null) _instance = FindObjectOfType<PoolManager>(); return _instance; } }

    private void Awake()
    {
        _poolables = new List<IPoolable>();
    }

    //Returns first deactivated poolable of chosen type. If none, instanciate one
    public T GetPoolable<T>(Vector3 position, Quaternion rotation) where T : IPoolable 
    {
        var poolable = (T)_poolables.FirstOrDefault(p => p is T && !((MonoBehaviour)p).gameObject.activeSelf);

        if (poolable == null)
        {
            var poolableModel = _poolableModels.FirstOrDefault(p => p.GetComponent<IPoolable>() is T);

            if (poolableModel == null)
                return default;

            poolable = Instantiate(poolableModel, _poolableParent).GetComponent<T>();
            _poolables.Add(poolable);
        }

        poolable.Pool(position, rotation);

        return poolable;
    }
}