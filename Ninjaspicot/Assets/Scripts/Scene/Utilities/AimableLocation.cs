using UnityEngine;

public class AimableLocation : MonoBehaviour, IFocusable
{
    private Hero _hero;
    public Hero Hero { get { if (_hero == null) _hero = Hero.Instance; return _hero; } }
    public bool IsSilent => true;


    private Transform _transform;
    public Transform Transform { get { if (_transform == null) _transform = transform; return _transform; } }

    public bool Active { get; set; }
    public bool Taken => !Active;
}
