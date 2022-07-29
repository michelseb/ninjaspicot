using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Abstract;
using ZepLink.RiceNinja.Dynamics.Scenery.Zones;
using ZepLink.RiceNinja.Interfaces;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Traps.Lasers.Components
{
    public class LaserEnd : Dynamic, ISceneryWakeable, IResettable
    {
        [SerializeField] private float _amplitude;
        [SerializeField] private float _speed;
        [SerializeField] private int _direction;

        public bool IsDynamic => _amplitude != 0 && _speed != 0;
        
        private Zone _zone;
        public Zone Zone { get { if (BaseUtils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

        private bool _active;
        private Vector3 _initPosition;
        private bool _wentThroughCenter;
        private Vector3 _directionVector;

        protected virtual void Start()
        {
            _initPosition = Transform.position;
            _wentThroughCenter = true;
        }


        protected virtual void Update()
        {
            if (!_active)
                return;

            if (!IsDynamic)
                return;

            _wentThroughCenter = _wentThroughCenter || Vector3.Dot(_initPosition - Transform.position, _directionVector) < 0;

            if (_wentThroughCenter && Vector3.Distance(Transform.position, _initPosition) >= _amplitude)
            {
                _direction = -_direction;
                _wentThroughCenter = false;
                _directionVector = _initPosition - Transform.position;
            }

            Transform.Translate(_speed * Mathf.Sign(_direction) * Time.deltaTime, 0, 0);
        }



        public void DoReset()
        {
            Transform.position = _initPosition;
            Wake();
        }

        public void Sleep()
        {
            _active = false;
        }

        public void Wake()
        {
            _active = true;
        }
    }
}