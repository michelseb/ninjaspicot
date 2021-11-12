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
    public T GetPoolable<T>(Vector3 position, Quaternion rotation, float size = 1f, PoolableType type = PoolableType.None, Transform parent = null, bool defaultParent = true) where T : IPoolable
    {
        T poolable = default(T);

        foreach (var p in Poolables.ToArray())
        {
            if (Utils.IsNull(p))
            {
                Poolables.Remove(p);
                continue;
            }

            if (type != PoolableType.None && p.PoolableType != type)
                continue;

            if (p is T && !((MonoBehaviour)p).gameObject.activeSelf)
            {
                poolable = (T)p;
                break;
            }
        }

        //var poolable = (T)Poolables.Where(type == PoolableType.None || x.PoolableType == type).FirstOrDefault(p => p is T && !((MonoBehaviour)p).gameObject.activeSelf);

        if (poolable != null)
            return SelectPoolable(poolable, position, rotation, size);

        // Create poolable if doesn't exist
        GameObject poolableModel = null;

        foreach (var model in _poolableModels)
        {
            if (model.TryGetComponent(out IPoolable pool) && pool is T && (type == PoolableType.None || pool.PoolableType == type))
            {
                poolableModel = model;
                break;
            }
        }

        if (poolableModel == null)
            return default;

        if (defaultParent)
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

        return SelectPoolable(poolable, position, rotation, size);
    }

    public T SelectPoolable<T>(T poolable, Vector3 position, Quaternion rotation, float size = 1f) where T : IPoolable
    {
        poolable.Wake();
        poolable.Pool(position, rotation, size);

        return poolable;
    }
}