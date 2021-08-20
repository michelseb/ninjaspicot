using UnityEngine;

public class DirectionIndicator : MonoBehaviour, IActivable
{
    private Hero _hero;
    public Hero Hero { get { if (_hero == null) _hero = Hero.Instance; return _hero; } }
    private Transform _transform;

    protected virtual void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    protected virtual void Start()
    {
        _transform = transform;
    }

    protected virtual void Update()
    {
        _transform.position = Hero.Transform.position;
        _transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, Utils.GetAngleFromVector(Hero.Stickiness.CollisionNormal) - 90), 10 * Time.deltaTime);
    }

    public virtual void Activate()
    {
    }

    public virtual void Deactivate()
    {
    }
}
