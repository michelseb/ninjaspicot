using System.Collections;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Interfaces;
using ZepLink.RiceNinja.Manageables.Audios;
using ZepLink.RiceNinja.ServiceLocator.Services.Abstract;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class TimeService : GameService, ITimeService, IActivable
    {
        public float TimeScale => Time.timeScale;

        private readonly IAudioService _audioService;

        private AudioFile _slowDown;
        private bool _active;

        private Coroutine _setTimeScale;

        private const float DEFAULT_INTERPOLATION = .5f;
        private const float DEFAULT_SLOWDOWN_TIMESCALE = .01f;
        private const float VOLUME_SLOWDOWN = .05f;

        public MonoBehaviour ServiceBehaviour { get; private set; }

        public TimeService(IAudioService audioService)
        {
            _audioService = audioService;
        }

        public override void Init(Transform parent)
        {
            base.Init(parent);

            _slowDown = _audioService.FindByName("SlowDown");

            ServiceBehaviour = ServiceObject.AddComponent<ServiceBehaviour>();

            Activate();
        }

        public void SlowDownImmediate()
        {
            SetTimeScaleImmediate(DEFAULT_SLOWDOWN_TIMESCALE);
        }

        public void SlowDownProgressive()
        {
            SetTimeScaleProgressive(DEFAULT_SLOWDOWN_TIMESCALE);
        }

        public void RestoreProgressive()
        {
            SetTimeScaleProgressive(1);
        }

        public void SlowDownAndRestoreProgressive()
        {
            SlowDownImmediate();
            RestoreProgressive();
        }

        private IEnumerator SetProgressive(float targetTimeScale, float delay)
        {
            var timeScale = Time.timeScale;
            var initialTimeScale = timeScale;
            var interpolation = 0f;

            while (interpolation < delay)
            {
                interpolation += Time.unscaledDeltaTime;
                timeScale = Mathf.Lerp(initialTimeScale, targetTimeScale, interpolation);
                SetTimeScale(timeScale);

                yield return null;
            }

            SetTimeScale(targetTimeScale);
            _setTimeScale = null;
        }

        public void SetTimeScaleProgressive(float timeScale, float delay)
        {
            if (!_active)
                return;

            _setTimeScale = ServiceBehaviour.StartCoroutine(SetProgressive(timeScale, delay));
        }

        public void SetTimeScaleImmediate(float timeScale)
        {
            if (!_active)
                return;

            _audioService.SetGlobalVolumeProgressive(VOLUME_SLOWDOWN, 0);
            _audioService.PlayGlobal(_slowDown);
            SetTimeScale(timeScale);
        }

        public void SetTimeScaleProgressive(float timeScale)
        {
            SetTimeScaleProgressive(timeScale, DEFAULT_INTERPOLATION);
        }

        public void SetNormalTime()
        {
            if (_setTimeScale != null)
            {
                ServiceBehaviour.StopCoroutine(_setTimeScale);
            }

            SetTimeScale(1);
            _audioService.SetGlobalVolumeProgressive(1, DEFAULT_INTERPOLATION);
        }

        public void StopTime()
        {
            SetTimeScale(0);
        }

        public void Activate(IActivator activator = default)
        {
            _active = true;
        }

        public void Deactivate(IActivator activator = default)
        {
            _active = false;
        }

        private void SetTimeScale(float scale)
        {
            Time.timeScale = scale;
            Time.fixedDeltaTime = Time.timeScale * .02f;
        }
    }
}