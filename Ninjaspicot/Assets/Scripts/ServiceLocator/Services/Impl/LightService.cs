using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using ZepLink.RiceNinja.Dynamics.Effects.Lights;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class LightService : PoolService<LightEffect>, ILightService
    {

        private Volume _volume;

        private readonly ICoroutineService _coroutineService;
        private readonly ICameraService _cameraService;

        private IDictionary<int, Guid> _lightUpdateRoutines;


        public LightService(ICoroutineService coroutineService, ICameraService cameraService)
        {
            _coroutineService = coroutineService;
            _cameraService = cameraService;

            _lightUpdateRoutines = new Dictionary<int, Guid>();
            _volume = _cameraService.MainCamera.Transform.GetComponentInChildren<Volume>();
            //_volume.enabled = false;
        }

        protected override string ModelPath => "Lights";

        public AmbiantLight CurrentAmbiant { get; private set; }

        public void SetLightsActivation(bool active)
        {
            foreach (var light in Collection)
            {
                light.enabled = active;
            }
        }

        public void BrightenAmbiant()
        {
            if (BaseUtils.IsNull(CurrentAmbiant))
                return;

            StopPrevious(CurrentAmbiant.Id);

            _coroutineService.StartCoroutine(DoBrighten(CurrentAmbiant.Light, .2f), out Guid id);

            _lightUpdateRoutines[CurrentAmbiant.Id] = id;
            //CurrentAmbiant.Brighten();
        }

        public void DimmAmbiant()
        {
            if (BaseUtils.IsNull(CurrentAmbiant))
                return;

            StopPrevious(CurrentAmbiant.Id);

            _coroutineService.StartCoroutine(DoDimm(CurrentAmbiant.Light, 1f), out Guid id);

            _lightUpdateRoutines[CurrentAmbiant.Id] = id;
            //CurrentAmbiant.Dimm();
        }

        public void ChromaBlast()
        {
            _coroutineService.StartCoroutine(DoChromaBlast(.5f));
        }

        public void SetAmbiant(AmbiantLight ambiantLight)
        {
            CurrentAmbiant = ambiantLight;
        }

        private void StopPrevious(int lightId)
        {
            if (!_lightUpdateRoutines.ContainsKey(lightId))
                return;

            _coroutineService.StopCoroutine(_lightUpdateRoutines[lightId]);
        }

        private IEnumerator DoChromaBlast(float duration)
        {
            _volume.profile.TryGet(out ChromaticAberration chroma);
            chroma.active = true;
            var t = 0f;

            while (t < duration)
            {
                chroma.intensity.value = Mathf.Lerp(1f, 0, t / duration);
                t += Time.deltaTime;

                yield return null;
            }

            chroma.active = false;
        }

        private IEnumerator DoBrighten(Light2D light, float duration = 1)
        {
            var initialIntensity = light.intensity;

            var initialVolume = light.volumeIntensity;
            //_volume.enabled = true;
            //var targetVolume = Mathf.Clamp01(initialVolume * intensityFactor);
            //light.volumeIntensityEnabled = true;

            _volume.profile.TryGet(out Bloom bloom);
            var initBloom = bloom.intensity.value;
            bloom.active = true;

            var t = 0f;

            while (t < duration)
            {
                var interpolation = t / duration;
                light.intensity = Mathf.Lerp(initialIntensity, 2, interpolation);
                bloom.intensity.value = Mathf.Lerp(initBloom, 1, interpolation);
                //light.volumeIntensity = Mathf.Lerp(initialVolume, targetVolume, t);
                t += Time.deltaTime;

                yield return null;
            }
        }

        private IEnumerator DoDimm(Light2D light, float duration = 1)
        {
            var t = 0f;
            var initialIntensity = light.intensity;
            //_volume.enabled = false;
            //light.volumeIntensityEnabled = false;
            _volume.profile.TryGet(out Bloom bloom);
            var initBloom = bloom.intensity.value;

            while (t < duration)
            {
                var interpolation = t / duration;
                light.intensity = Mathf.Lerp(initialIntensity, 1, interpolation);
                bloom.intensity.value = Mathf.Lerp(initBloom, 0, interpolation);
                //light.volumeIntensity = Mathf.Lerp(initialVolume, targetVolume, t);
                t += Time.deltaTime;

                yield return null;
            }

            bloom.active = false;
        }
    }
}