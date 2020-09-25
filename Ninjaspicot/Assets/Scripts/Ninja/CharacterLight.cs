using UnityEngine;

public class CharacterLight : Lamp
{
    private Transform _transform;

    protected override void Awake()
    {
        base.Awake();
        _transform = transform;
    }

    protected virtual void LateUpdate()
    {
        _transform.rotation = Quaternion.identity;
    }

    public void SetColor(Color color)
    {
        Light.color = color;
    }
}
