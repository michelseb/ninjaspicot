using UnityEngine;

public class CharacterLight : Lamp, IPoolable
{
    private Transform _transform;

    public PoolableType PoolableType => PoolableType.None;

    protected override void Awake()
    {
        base.Awake();
        _transform = transform;
    }

    public void Pool(Vector3 position, Quaternion rotation, float size)
    {
        _transform.position = new Vector3(position.x, position.y, -5);
    }

    public void SetColor(Color color)
    {
        _light.color = color;
    }
}
