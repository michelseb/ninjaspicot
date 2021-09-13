using UnityEngine;

public class Laser : MonoBehaviour, IWakeable, IActivable, IRaycastable
{

    [SerializeField] protected RectTransform _start;
    [SerializeField] protected RectTransform _end;
    [SerializeField] protected bool _horizontal;

    protected LineRenderer _laser;
    protected PolygonCollider2D _collider;
    protected bool _active;
    protected int _pointsAmount;
    private AudioManager _audioManager;
    private Zone _zone;
    private Audio _electrocutionSound;
    public Zone Zone { get { if (Utils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

    private int _id;
    public int Id { get { if (_id == 0) _id = gameObject.GetInstanceID(); return _id; } }

    public bool Sleeping { get; set; }

    protected virtual void Awake()
    {
        _laser = GetComponent<LineRenderer>();
        _collider = GetComponent<PolygonCollider2D>();
        _pointsAmount = (int)((_end.position - _start.position).magnitude / 2);
        _audioManager = AudioManager.Instance;
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
            _laser.SetPosition(i, pos + transform.up * delta);
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
                transform.InverseTransformPoint(startCorners[2]),
                transform.InverseTransformPoint(startCorners[3]),
                transform.InverseTransformPoint(endCorners[3]),
                transform.InverseTransformPoint(endCorners[2])
            });
        }
        else
        {
            _collider.SetPath(0, new Vector2[]
            {
                transform.InverseTransformPoint(startCorners[1]),
                transform.InverseTransformPoint(startCorners[2]),
                transform.InverseTransformPoint(endCorners[1]),
                transform.InverseTransformPoint(endCorners[2])
            });
        }
    }

    public void Sleep()
    {
        _collider.enabled = false;
        _laser.enabled = false;
        _active = false;
    }

    public void Wake()
    {
        _collider.enabled = true;
        _laser.enabled = true;
        _active = true;
    }

    public void Activate()
    {
        Hero.Instance.Die(sound: _electrocutionSound, volume: .5f);
    }

    public void Deactivate() { }
}
