using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Abstract;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Dynamics.Scenery.Traps.Lasers.Components;
using ZepLink.RiceNinja.Dynamics.Scenery.Zones;
using ZepLink.RiceNinja.Interfaces;
using ZepLink.RiceNinja.Manageables.Audios;
using ZepLink.RiceNinja.ServiceLocator.Services;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Traps.Lasers
{
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
        private IAudioService _audioService;
        private Zone _zone;
        private AudioFile _electrocutionSound;
        protected bool _broken;
        protected bool _isDynamic;
        public Zone Zone { get { if (BaseUtils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

        private Vector3[] _laserPositions;

        protected virtual void Awake()
        {
            _startTransform = (RectTransform)_start.Transform;
            _endTransform = (RectTransform)_end.Transform;
            _laser = GetComponent<LineRenderer>();
            _collider = GetComponent<PolygonCollider2D>();
            _pointsAmount = (int)((_endTransform.position - _startTransform.position).magnitude / 2);
            _laserPositions = new Vector3[_pointsAmount];
            _audioService = ServiceFinder.Get<IAudioService>();

            if (_startAwake)
            {
                Wake();
            }
        }

        protected virtual void Start()
        {
            _laser.positionCount = _pointsAmount;
            _electrocutionSound = _audioService.FindByName("Electrocution");
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

            if (_isDynamic)
            {
                _startPosition = _startTransform.localPosition;
                _endPosition = _endTransform.localPosition;

                UpdateCollider();
            }

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
            if (collision.TryGetComponent(out IActivator activator))
            {
                Activate(activator);
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
        }

        public virtual void Wake()
        {
            if (_broken)
                return;

            _collider.enabled = true;
            _laser.enabled = true;
            _active = true;
        }

        public virtual void Activate(IActivator activator = default)
        {
            if (activator is not IKillable killable)
                return;

            killable.Die(sound: _electrocutionSound, volume: .5f);
        }

        public virtual void Deactivate(IActivator activator = default)
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
}