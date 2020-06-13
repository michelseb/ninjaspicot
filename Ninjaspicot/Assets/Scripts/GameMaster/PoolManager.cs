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
    public T GetPoolable<T>(Vector3 position, Quaternion rotation, PoolableType type) where T : IPoolable 
    {
        var poolable = (T)_poolables.Where(x => x.PoolableType == type).FirstOrDefault(p => p is T && !((MonoBehaviour)p).gameObject.activeSelf);

        if (poolable == null)
        {
            GameObject poolableModel = null;

            foreach (var model in _poolableModels)
            {
                var pool = model.GetComponent<IPoolable>();
                if (pool == null)
                    continue;

                if (pool is T && pool.PoolableType == type)
                {
                    poolableModel = model;
                    break;
                }
            }

            if (poolableModel == null)
                return default;

            poolable = Instantiate(poolableModel, _poolableParent).GetComponent<T>();
            _poolables.Add(poolable);
        }

        poolable.Pool(position, rotation);

        return poolable;
    }
}