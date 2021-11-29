using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Laser : Dynamic, ISceneryWakeable, IActivable, IRaycastable, IResettable
{

    [SerializeField] protected LaserEnd _start;
    [SerializeField] protected LaserEnd _end;
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
    protected RectTransform _startTransform, _endTransform;
    protected Vector3 _startPosition, _endPosition;
    private AudioManager _audioManager;
    private Zone _zone;
    private Audio _electrocutionSound;
    protected bool _broken;
    protected bool _isDynamic;
    protected ParticleSystem _activationIndicator;
    public Zone Zone { get { if (Utils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

    private int _id;
    public int Id { get { if (_id == 0) _id = gameObject.GetInstanceID(); return _id; } }

    private Vector3[] _laserPositions;

    protected virtual void Awake()
    {
        _startTransform = (RectTransform)_start.Transform;
        _endTransform = (RectTransform)_end.Transform;
        _laser = GetComponent<LineRenderer>();
        _collider = GetComponent<PolygonCollider2D>();
        _pointsAmount = (int)((_endTransform.position - _startTransform.position).magnitude / 2);
        _laserPositions = new Vector3[_pointsAmount];
        _audioManager = AudioManager.Instance;
        _activationIndicator = GetComponentInChildren<ParticleSystem>();
        if (_startAwake)
        {
            Wake();
        }
    }

    protected virtual void Start()
    {
        _laser.positionCount = _pointsAmount;
        _electrocutionSound = _audioManager.FindAudioByName("Electrocution");
        _startPosition = _startTransform.localPosition;
        _endPosition = _endTransform.localPosition;
        _isDynamic = _start.IsDynamic || _end.IsDynamic;

        UpdateCollider();
        InitPointsPosition();
        SetPointsPosition();
    }

    protected virtual void Update()
    {
        if (!_active)
            return;

        var hit = CollisionPoint();
        _startPosition = _startTransform.localPosition;
        _endPosition = hit ? _endTransform.InverseTransformPoint(hit.point) : _endTransform.localPosition;

        UpdateCollider();


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
        _laser.SetPosition(0, _startPosition + _startTransform.right);
        _laser.SetPosition(_pointsAmount - 1, _endPosition + _endTransform.right);

        for (int i = 1; i < _pointsAmount - 1; i++)
        {
            var pos = _startPosition + ((_endPosition - _startPosition) * (i + 1) / _pointsAmount);
            _laser.SetPosition(i, pos);
        }
    }

    protected RaycastHit2D CollisionPoint()
    {
        return Utils.LineCast(_startTransform.position, _endTransform.position);
    }

    protected virtual void SetPointsPosition()
    {
        if (_isDynamic)
        {
            _laser.SetPosition(0, _startPosition + _startTransform.right);
            _laser.SetPosition(_pointsAmount - 1, _endPosition + _endTransform.right);
        }

        var delta = Random.Range(-_width, _width); // -1 or 1 * width
        var direction = (_endPosition - _startPosition).normalized;
        var normal = new Vector3(direction.y, -direction.x);

        for (int i = 1; i < _pointsAmount - 1; i++)
        {
            var mid = _startPosition + ((_endPosition - _startPosition) * (i + 1) / _pointsAmount);
            _laserPositions[i] = mid + normal * delta;
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

    protected virtual void UpdateCollider()
    {
        var startCorners = new Vector3[4];
        var endCorners = new Vector3[4];
        _startTransform.GetWorldCorners(startCorners);
        _endTransform.GetWorldCorners(endCorners);

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
        _activationIndicator.Stop();
    }

    public virtual void Wake()
    {
        if (_broken)
            return;

        _collider.enabled = true;
        _laser.enabled = true;
        _active = true;
        _activationIndicator.Play();
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
