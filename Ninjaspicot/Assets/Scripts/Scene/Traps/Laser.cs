using UnityEngine;

public class Laser : MonoBehaviour, ISceneryWakeable, IActivable, IRaycastable, IResettable
{

    [SerializeField] protected RectTransform _start;
    [SerializeField] protected RectTransform _end;
    [SerializeField] protected bool _horizontal;
    [SerializeField] protected bool _startAwake;
    [SerializeField] protected int _updateTime;
    [SerializeField] protected float _width;
    [SerializeField] protected float _amplitude;
    [SerializeField] protected float _amplitudeTurbulance;
    [SerializeField] protected float _variation;

    protected LineRenderer _laser;
    protected PolygonCollider2D _collider;
    protected bool _active;
    protected int _pointsAmount;
    protected Vector3 _startPosition, _endPosition;
    private AudioManager _audioManager;
    private Zone _zone;
    private Audio _electrocutionSound;
    private bool _broken;
    public Zone Zone { get { if (Utils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

    private int _id;
    public int Id { get { if (_id == 0) _id = gameObject.GetInstanceID(); return _id; } }

    private Transform _transform;
    public Transform Transform { get { if (Utils.IsNull(_transform)) _transform = transform; return _transform; } }

    private Vector3[] _laserPositions;

    protected virtual void Awake()
    {
        _laser = GetComponent<LineRenderer>();
        _collider = GetComponent<PolygonCollider2D>();
        _pointsAmount = (int)((_end.position - _start.position).magnitude / 2);
        _laserPositions = new Vector3[_pointsAmount];
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
        _startPosition = _start.localPosition;
        _endPosition = _end.localPosition;

        SetCollider();
        InitPointsPosition();
        SetPointsPosition();
    }

    protected virtual void Update()
    {
        if (!_active)
            return;

        if (Time.frameCount % _updateTime == 0)
        {
            SetPointsPosition();
        }

        MovePoints();
    }

    protected virtual void MovePoints()
    {
        for (int i = 1; i < _pointsAmount - 1; i++)
        {
            var pointAt = _laser.GetPosition(i);
            pointAt = Vector3.Lerp(pointAt, _laserPositions[i], Time.deltaTime * 10);
            _laser.SetPosition(i, pointAt);
        }
    }

    protected virtual void InitPointsPosition()
    {
        _laser.SetPosition(0, _startPosition + _start.right);
        _laser.SetPosition(_pointsAmount - 1, _endPosition + _end.right);

        for (int i = 1; i < _pointsAmount - 1; i++)
        {
            var pos = _startPosition + ((_endPosition - _startPosition) * (i + 1) / _pointsAmount);
            _laser.SetPosition(i, pos);
        }
    }

    protected virtual void SetPointsPosition()
    {
        var delta = Random.Range(-_width, _width); // -1 or 1 * width

        for (int i = 1; i < _pointsAmount - 1; i++)
        {
            var mid = _startPosition + ((_endPosition - _startPosition) * (i + 1) / _pointsAmount);
            _laserPositions[i] = mid + Vector3.up * delta;
            delta *= Random.Range(-_amplitude - _amplitudeTurbulance, -_amplitude + _amplitudeTurbulance);
            delta += Random.Range(-_variation, _variation);
            delta = Mathf.Clamp(delta, -_width, _width);
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
