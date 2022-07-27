using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Effects.Sounds;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Interfaces;
using ZepLink.RiceNinja.ServiceLocator.Services;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Characters.Components.Hearing
{
    public abstract class HearingPerimeter : Dynamic, IActivable
    {
        public virtual float Size => _listener?.Range ?? 1f;
        public SoundMark SoundMark { get; private set; }

        protected IListener _listener;
        protected IPoolService _poolManager;
        protected bool _isActive;

        protected virtual void Awake()
        {
            _poolManager = ServiceFinder.Get<IPoolService>();
            _listener = GetComponent<IListener>() ?? GetComponentInChildren<IListener>() ?? GetComponentInParent<IListener>();
        }

        protected virtual void Start()
        {
            Transform.localScale = Vector3.one * Size;
        }

        protected virtual void OnTriggerEnter2D(Collider2D collider) // Before => Stay2D
        {
            if (!collider.CompareTag("Sound") || !_isActive)
                return;

            _listener.Hear(new HearingArea
            {
                SourcePoint = collider.transform.position
            });

            EraseSoundMark();
            SoundMark = _poolManager.GetPoolable<SoundMark>(collider.transform.position, Quaternion.identity);
        }

        public void Activate(IActivator activator = default)
        {
            _isActive = true;
            gameObject.SetActive(true);
        }
        public void Deactivate(IActivator activator = default)
        {
            _isActive = false;
            gameObject.SetActive(false);
        }

        public void EraseSoundMark()
        {
            if (BaseUtils.IsNull(SoundMark))
                return;

            SoundMark.Sleep();
            SoundMark = null;
        }
    }
}