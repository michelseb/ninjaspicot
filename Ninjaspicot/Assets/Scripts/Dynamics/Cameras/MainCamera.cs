using System.Collections;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Abstract;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Interfaces;
using ZepLink.RiceNinja.ServiceLocator.Services;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Cameras
{
    public class MainCamera : Dynamic, ICamera, IActivable
    {
        [SerializeField] private int _beginZoom;

        private Vector3 _centerPos;
        private Vector3 _velocity;

        private const float ZOOM_SPEED = 2f;
        private const float FOLLOW_DELAY = 0.8f;
        private const float OFFSET_ADJUST_DELAY = 1.8f;

        private Camera _camera;
        public Camera Camera { get { if (BaseUtils.IsNull(_camera)) _camera = GetComponent<Camera>(); return _camera; } }

        public float Size { get { return Camera.orthographicSize; } private set { Camera.orthographicSize = value; } }

        private Transform _parentTransform;
        public override Transform Transform { get { if (BaseUtils.IsNull(_parentTransform)) _parentTransform = transform.parent.transform; return _parentTransform; } }
        public CameraMode CurrentMode { get; private set; }

        public ITracker CurrentTracker { get; private set; }

        private ICoroutineService _coroutineService;
        public ICoroutineService CoroutineService { get { if (BaseUtils.IsNull(_coroutineService)) _coroutineService = ServiceFinder.Get<ICoroutineService>(); return _coroutineService; } }

        private float _screenRatio;
        private float _initialCamSize;

        private void Awake()
        {
            Deactivate();
        }

        private void Start()
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;

            _screenRatio = (float)Screen.height / Screen.width * .5f;
            _initialCamSize = 17.85f * _screenRatio;

            _velocity = Vector3.zero;
            InstantZoom(_beginZoom);
            //SetFollowMode(_tracker);
            Zoom(ZoomType.Intro);
        }

        private void Update()
        {
            switch (CurrentMode)
            {
                case CameraMode.Follow:
                    Follow();
                    break;

                case CameraMode.Center:
                    Center(CurrentTracker, _centerPos);
                    break;
            }
        }

        /// <summary>
        /// Get touch position in world coords
        /// </summary>
        /// <param name="screenPoint"></param>
        /// <returns></returns>
        public Vector2 ScreenToWorldPoint(Vector2 screenPoint)
        {
            return Camera.ScreenToWorldPoint(screenPoint);
        }

        /// <summary>
        /// Get touch position in world coords
        /// </summary>
        /// <param name="screenPoint"></param>
        /// <returns></returns>
        public Vector2 WorldToScreenPoint(Vector2 screenPoint)
        {
            return Camera.WorldToScreenPoint(screenPoint);
        }

        //private void Update()
        //{
        //    if (_hero == null)
        //    {
        //        _hero = Hero.Instance;
        //        _heroTransform = _hero?.Transform;
        //        _heroStickiness = _hero?.Stickiness;
        //        ParentTransform.position = _heroTransform.position;
        //        var background = FindObjectOfType<Background>();
        //        if (background != null)
        //        {
        //            background.CenterBackground();
        //        }
        //    }
        //}

        private void Follow(ITracker tracker)
        {
            //float speed;

            //if (_heroStickiness.Walking || !_heroStickiness.Attached)
            //{
            //    speed = FOLLOW_DELAY;
            //}
            //else
            //{
            //    _normalOffset = Quaternion.Euler(0, 0, 90) * _heroStickiness.CollisionNormal * 25;
            //    speed = OFFSET_ADJUST_DELAY;
            //}
            ////Debug.Log(_normalOffset);
            //if (!_heroStickiness.Attached || _normalOffset.magnitude > 0 && Vector3.Dot(_normalOffset, Quaternion.Euler(0, 0, 90) * _heroStickiness.CollisionNormal * 25) < .5f)
            //{
            //    _normalOffset = Vector3.zero;
            //}

            var offset = tracker.NormalVector * 2.5f;

            Transform.position = Vector3.SmoothDamp(Transform.position, tracker.Transform.position + offset, ref _velocity, FOLLOW_DELAY);
        }

        private void Follow()
        {
            if (CurrentTracker == null)
                return;

            Follow(CurrentTracker);
        }

        public void MoveTo(Vector3 pos)
        {
            Transform.position = new Vector3(pos.x, pos.y, Transform.position.z);
        }

        private void Center(ITracker tracker)
        {
            Center(tracker?.Transform.position ?? Vector2.zero);
        }

        private void Center(ITracker tracker, Vector3 middle)
        {
            var trackerPosition = tracker?.Transform.position ?? Vector2.zero;
            var center = (trackerPosition + middle) / 2;

            Center(center);
        }

        private void CenterImmediate(ITracker target)
        {
            Transform.position = target?.Transform.position ?? Vector3.zero;
        }

        private void Center(Vector3 target)
        { 
            Transform.position = Vector3.SmoothDamp(Transform.position, target, ref _velocity, FOLLOW_DELAY);
        }

        public void SetFollowMode()
        {
            CurrentMode = CameraMode.Follow;
            _velocity = Vector3.zero;
        }

        public void SetCenterMode(Vector3 centerPos)
        {
            CurrentMode = CameraMode.Center;
            _centerPos = new Vector3(centerPos.x, centerPos.y, Transform.position.z);
        }

        private IEnumerator ZoomProgressive(int zoom)
        {
            float _initSize = Size;

            while (Mathf.Abs(Size - _initSize) < Mathf.Abs(zoom * _screenRatio))
            {
                Size -= zoom * Time.unscaledDeltaTime * ZOOM_SPEED * _screenRatio;
                yield return null;
            }
        }

        private IEnumerator ReinitZoom()
        {
            var delta = _initialCamSize - Size;

            while (Mathf.Sign(delta) * (_initialCamSize - Size) > 0)
            {
                Size += delta * Time.unscaledDeltaTime * ZOOM_SPEED * _screenRatio;
                yield return null;
            }

            Size = _initialCamSize;
        }

        private IEnumerator ZoomIntro(float speed)
        {
            CenterImmediate(CurrentTracker);
            Activate();

            yield return new WaitForSecondsRealtime(2);

            while (Size > _initialCamSize)
            {
                Size -= speed;
                yield return null;
            }
            SetFollowMode();
            //_hero.Jumper.Active = true;
        }

        private void InstantZoom(float zoom)
        {
            Size = _initialCamSize + zoom * _screenRatio;
        }

        public void Zoom(ZoomType type, int zoomAmount = 0)
        {
            StopAllCoroutines();

            switch (type)
            {
                case ZoomType.Progressive:
                    CoroutineService.StartCoroutine(ZoomProgressive(zoomAmount));
                    break;

                case ZoomType.Instant:
                    InstantZoom(zoomAmount);
                    break;

                case ZoomType.Intro:
                    CoroutineService.StartCoroutine(ZoomIntro(ZOOM_SPEED));
                    break;

                case ZoomType.Init:
                    CoroutineService.StartCoroutine(ReinitZoom());
                    break;
            }
        }

        public void Shake(float duration, float strength)
        {
            _coroutineService.StartCoroutine(DoShake(duration, strength));
        }

        private IEnumerator DoShake(float duration, float strength)
        {
            var pos = transform.localPosition;

            float ellapsed = 0;

            while (ellapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * strength;
                float y = Random.Range(-1f, 1f) * strength;

                transform.localPosition = new Vector3(x, y, pos.z);

                ellapsed += Time.deltaTime;
                yield return null;
            }

            Transform.localPosition = pos;
        }

        public void SetTracker(ITracker tracker)
        {
            CurrentTracker = tracker;
        }

        public void SetMode(CameraMode cameraMode)
        {
            CurrentMode = cameraMode;
        }

        public void Activate(IActivator activator = null)
        {
            Camera.enabled = true;
        }

        public void Deactivate(IActivator activator = null)
        {
            Camera.enabled = false;
        }
    }
}