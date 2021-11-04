using UnityEngine;

public class Generator : MonoBehaviour, IActivable, ISceneryWakeable, IRaycastable, IFocusable, IResettable
{
    [SerializeField] protected Laser[] _lasers;
    
    private Zone _zone;
    public Zone Zone { get { if (Utils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

    private int _id;
    public int Id { get { if (_id == 0) _id = gameObject.GetInstanceID(); return _id; } }

    public bool IsSilent => false;
    public bool Taken => false;

    public bool FocusedByNormalJump => false;

    private Lamp _light;

    private void Awake()
    {
        _light = GetComponentInChildren<Lamp>();
    }

    public void Activate() 
    {
        Sleep();

        foreach (var laser in _lasers)
        {
            laser.Deactivate();
        }
    }

    public void Deactivate() { }

    public void DoReset()
    {
        Wake();

        foreach (var laser in _lasers)
        {
            laser.DoReset();
        }
    }

    public void Sleep()
    {
        _light.Sleep();
    }

    public void Wake()
    {
        _light.Wake();
    }
}
