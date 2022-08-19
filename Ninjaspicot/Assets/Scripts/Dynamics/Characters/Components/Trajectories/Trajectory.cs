using System.Collections;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Abstract;
using ZepLink.RiceNinja.Dynamics.Effects;
using ZepLink.RiceNinja.Dynamics.Effects.Sounds;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Helpers;
using ZepLink.RiceNinja.ServiceLocator.Services;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Characters.Components
{
    public abstract class Trajectory : Dynamic, IPoolable
    {
        public bool Used { get; protected set; }
        public bool Active { get; protected set; }
        public float Strength { get; set; }
        public IFocusable Focusable { get; protected set; }

        public abstract CustomColor Color { get; }
        public abstract JumpMode JumpMode { get; }

        protected LineRenderer _line;
        protected AimIndicator _aimIndicator;
        protected SimulatedSoundEffect _audioSimulator;
        protected AnimationCurve _lineWidth;

        protected ICoroutineService _coroutineService;

        protected virtual float _fadeSpeed => .5f;
        protected const int MAX_VERTEX = 50; //50
        protected const float LENGTH = .04f;

        protected virtual void Awake()
        {
            _coroutineService = ServiceFinder.Get<ICoroutineService>();

            _line = GetComponent<LineRenderer>();
            _lineWidth = _line.widthCurve;
        }

        protected virtual void OnEnable()
        {
            Active = true;
        }

        public abstract void DrawTrajectory(Vector2 linePosition, Vector2 direction);

        protected virtual RaycastHit2D StepClear(Vector3 origin, Vector3 direction, float distance)
        {
            // Readapt radius if hero scale changes (otherwise cast hits the ground behind hero)
            return Physics2D.CircleCast(origin, .05f, direction, distance,
                        (1 << LayerMask.NameToLayer("Obstacle")) |
                        (1 << LayerMask.NameToLayer("DynamicObstacle")) |
                        (1 << LayerMask.NameToLayer("Enemy")) |
                        (1 << LayerMask.NameToLayer("Interactive")) |
                        (1 << LayerMask.NameToLayer("Teleporter")));
        }

        protected virtual RaycastHit2D StepClearWall(Vector3 origin, Vector3 direction, float distance)
        {
            // Readapt radius if hero scale changes (otherwise cast hits the ground behind hero)
            return Physics2D.CircleCast(origin, .05f, direction, distance,
                        (1 << LayerMask.NameToLayer("Obstacle")) |
                        (1 << LayerMask.NameToLayer("DynamicObstacle")));
        }

        public virtual void StartFading()
        {
            Used = false;

            _coroutineService.StartCoroutine(FadeAway());
            DeactivateAim();
        }

        protected virtual IEnumerator FadeAway()
        {
            _audioSimulator?.Sleep();
            _audioSimulator = null;

            var col = _line.material.color;

            while (col.a > 0)
            {
                col = _line.material.color;
                col.a -= Time.deltaTime * _fadeSpeed;
                _line.material.color = col;
                yield return null;
            }

            Active = false;
            Sleep();
        }

        public virtual void ReUse(Vector3 position)
        {
            Transform.position = new Vector3(position.x, position.y, -5);
            Appear();
        }

        protected virtual void Appear()
        {
            if (Used)
                return;

            Color col = _line.material.color;
            _line.material.color = new Color(col.r, col.g, col.b, 1);

            Used = true;
        }

        public void Pool(Vector3 position, Quaternion rotation, float size)
        {
            Transform.position = new Vector3(position.x, position.y, -5);
            Transform.rotation = rotation;
            Appear();
        }

        public void Sleep()
        {
            gameObject.SetActive(false);
        }

        public void Wake()
        {
            gameObject.SetActive(true);
        }

        public void SetAudioSimulator(Vector3 position, float size)
        {
            if (BaseUtils.IsNull(_audioSimulator))
            {
                _audioSimulator = PoolHelper.Pool<SimulatedSoundEffect>(position, Quaternion.identity, size);
            }

            _audioSimulator.Transform.position = position;
            _audioSimulator.Transform.localScale = size * Vector2.one;
        }

        protected virtual void ActivateAim(IFocusable focusable, Vector3 position)
        {
            if (_aimIndicator != null)
                return;

            Focusable = focusable;
            _aimIndicator = PoolHelper.Pool<AimIndicator>(position, Quaternion.identity);
        }

        protected virtual void DeactivateAim()
        {
            if (_aimIndicator == null)
                return;

            _aimIndicator.Sleep();
            _aimIndicator = null;
            Focusable = null;
        }

        public Vector3[] GetPositions()
        {
            Vector3[] result = new Vector3[_line.positionCount];

            for (int a = 0; a < _line.positionCount; a++)
            {
                result[a] = _line.GetPosition(a);
            }

            return result;
        }

        public Vector3 GetLastPosition()
        {
            return _line.GetPosition(_line.positionCount - 1);
        }

        protected virtual void ResetWidths()
        {
            _line.widthCurve = _lineWidth;
        }

        public void DoReset()
        {
            Sleep();
        }
    }
}