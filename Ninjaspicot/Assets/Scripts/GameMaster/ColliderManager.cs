using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColliderManager : MonoBehaviour
{
    private CompositeContainer _compositeContainer;
    private static ColliderManager _instance;
    public static ColliderManager Instance { get { if (_instance == null) _instance = FindObjectOfType<ColliderManager>(); return _instance; } }

    private List<Transform> _walls;

    public void InitCompositeColliders()
    {
        _compositeContainer = PoolManager.Instance.GetPoolable<CompositeContainer>(Vector3.zero, Quaternion.identity, PoolableType.None, false);

        _walls = GameObject.FindGameObjectsWithTag("Wall")
            .Select(wall => wall.transform).ToList();

        _walls.ForEach(wall => wall.SetParent(_compositeContainer.transform));

    }
}