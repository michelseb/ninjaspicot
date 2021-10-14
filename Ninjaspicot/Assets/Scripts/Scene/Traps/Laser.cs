using UnityEngine;

public class Laser : MonoBehaviour, ISceneryWakeable, IActivable, IRaycastable, IResettable
{

    [SerializeField] protected RectTransform _start;
    [SerializeField] protected RectTransform _end;
    [SerializeField] protected bool _horizontal;
    [SerializeField] protected bool _startAwake;

    protected LineRenderer _laser;
    protected PolygonCollider2D _collider;
    protected bool _active;
    protected int _pointsAmount;
    private AudioManager _audioManager;
    private Zone _zone;
    private Audio _electrocutionSound;
    private bool _broken;
    public Zone Zone { get { if (Utils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

    private int _id;
    public int Id { get { if (_id == 0) _id = gameObject.GetInstanceID(); return _id; } }

    private Transform _transform;
    public Transform Transform { get { if (Utils.IsNull(_transform)) _transform = transform; return _transform; } }

    protected virtual void Awake()
    {
        _laser = GetComponent<LineRenderer>();
        _collider = GetComponent<PolygonCollider2D>();
        _pointsAmount = (int)((_end.position - _start.position).magnitude / 2);
        _audioManager = AudioManager.Instance;
        if (_startAwake)
        {
            Wake();
        }
    }

    protected virtual void Start()
    {
        _laser.positionCount = _pointsAmount;
        _electrocutionSound = _audioManager.FindAudioByName("Electrocution");

        SetCollider();
        InitPointsPosition();
    }

    protected virtual void Update()
    {
        if (!_active)
            return;

        if (Time.frameCount % Utils.EXPENSIVE_FRAME_INTERVAL == 0)
        {
            SetPointsPosition();
        }
    }

    protected virtual void InitPointsPosition()
    {
        _laser.SetPosition(0, _start.position + _start.right);
        _laser.SetPosition(_pointsAmount - 1, _end.position + _end.right);
        SetPointsPosition();
    }

    protected virtual void SetPointsPosition()
    {
        var delta = Random.Range(0, 2) * 2 - 1; // -1 or 1

        for (int i = 1; i < _pointsAmount - 1; i++)
        {
            var pos = _start.position + ((_end.position - _start.position) * (i + 1) / _pointsAmount);
            _laser.SetPosition(i, pos + Transform.up * delta);
            delta *= -1;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("hero"))
        {
            Activate();
        }
    }

    protected virtual void SetCollider()
    {
        var startCorners = new Vector3[4];
        var endCorners = new Vector3[4];
        _start.GetWorldCorners(startCorners);
        _end.GetWorldCorners(endCorners);

        if (_horizontal)
        {
            _collider.SetPath(0, new Vector2[]
            {
                Transform.InverseTransformPoint(startCorners[2]),
                Transform.InverseTransformPoint(startCorners[3]),
                Transform.InverseTransformPoint(endCorners[3]),
                Transform.InverseTransformPoint(endCorners[2])
            });
        }
        else
        {
            _collider.SetPath(0, new Vector2[]
            {
                Transform.InverseTransformPoint(startCorners[1]),
                Transform.InverseTransformPoint(startCorners[2]),
                Transform.InverseTransformPoint(endCorners[1]),
                Transform.InverseTransformPoint(endCorners[2])
            });
        }
    }

    public virtual void Sleep()
    {
        _collider.enabled = false;
        _laser.enabled = false;
        _active = false;
    }

    public virtual void Wake()
    {
        if (_broken)
            return;

        _collider.enabled = true;
        _laser.enabled = true;
        _active = true;
    }

    public virtual void Activate()
    {
        Hero.Instance.Die(sound: _electrocutionSound, volume: .5f);
    }

    public virtual void Deactivate()
    {
        _broken = true;
        Sleep();
    }

    public void DoReset()
    {
        _broken = false;
        Wake();
    }
}
