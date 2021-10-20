using UnityEngine;

public class AimIndicator : MonoBehaviour, IPoolable
{
    private Hero _hero;
    public Hero Hero { get { if (_hero == null) _hero = Hero.Instance; return _hero; } }

    public PoolableType PoolableType => PoolableType.None;

    private Transform _transform;
    public Transform Transform { get { if (Utils.IsNull(_transform)) _transform = transform; return _transform; } }

    protected virtual void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    protected virtual void Update()
    {
        Transform.Rotate(0, 0, 1);
    }

    public virtual void Wake()
    {
        gameObject.SetActive(true);
    }

    public virtual void Sleep()
    {
        gameObject.SetActive(false);
    }

    public void Pool(Vector3 position, Quaternion rotation, float size = 1)
    {
        Transform.position = position;
        Transform.localScale = Vector3.one * size;
    }

    public void DoReset()
    {
        Sleep();
    }
}
