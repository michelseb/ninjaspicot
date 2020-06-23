using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _poolableModels;
    [SerializeField] private Transform _poolableParent;

    private List<IPoolable> _poolables;
    public List<IPoolable> Poolables { get { if (_poolables == null) _poolables = new List<IPoolable>(); return _poolables; } }

    private static PoolManager _instance;
    public static PoolManager Instance { get { if (_instance == null) _instance = FindObjectOfType<PoolManager>(); return _instance; } }

    //Returns first deactivated poolable of chosen type. If none, instanciate one
    public T GetPoolable<T>(Vector3 position, Quaternion rotation, PoolableType type = PoolableType.None, Transform parent = null) where T : IPoolable
    {
        var poolable = (T)Poolables.Where(x => type == PoolableType.None || x.PoolableType == type).FirstOrDefault(p => p is T && !((MonoBehaviour)p).gameObject.activeSelf);

        if (poolable == null)
        {
            GameObject poolableModel = null;

            foreach (var model in _poolableModels)
            {
                var pool = model.GetComponent<IPoolable>();
                if (pool == null)
                    continue;

                if (pool is T && (type == PoolableType.None || pool.PoolableType == type))
                {
                    poolableModel = model;
                    break;
                }
            }

            if (poolableModel == null)
                return default;

            if (parent == null)
            {
                poolable = Instantiate(poolableModel, _poolableParent).GetComponent<T>();
            }
            else
            {
                var poolableObject = Instantiate(poolableModel);
                poolableObject.transform.SetParent(parent, true);

                poolable = poolableObject.GetComponent<T>();
            }

            Poolables.Add(poolable);
        }

        poolable.Pool(position, rotation);

        return poolable;
    }
}