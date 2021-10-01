using UnityEngine;

public class Generator : MonoBehaviour, IActivable, IRaycastable, IFocusable
{
    [SerializeField] protected Laser[] _lasers;
    
    private Zone _zone;
    public Zone Zone { get { if (Utils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

    private int _id;
    public int Id { get { if (_id == 0) _id = gameObject.GetInstanceID(); return _id; } }

    public bool Sleeping { get; set; }

    private Lamp _light;

    private void Awake()
    {
        _light = GetComponentInChildren<Lamp>();
    }

    public void Activate() 
    {
        _light.Animator.enabled = false;
        _light.Light.enabled = false;

        foreach (var laser in _lasers)
        {
            laser.Deactivate();
        }
    }

    public void Deactivate() { }
}
