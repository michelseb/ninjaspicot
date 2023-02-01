using UnityEngine;
using UnityEngine.Rendering.Universal;
using ZepLink.RiceNinja.Dynamics.Abstract;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Helpers;
using ZepLink.RiceNinja.ServiceLocator.Services;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Effects.Lights
{
    public abstract class LightEffect : Dynamic, IPoolable
    {
        protected float _initIntensity;
        protected Color _initColor;

        protected ILightService _lightService;

        private Light2D _light;
        public Light2D Light
        {
            get { if (BaseUtils.IsNull(_light)) _light = GetComponentInChildren<Light2D>(); return _light; }
        }

        protected virtual void Awake()
        {
            _lightService = ServiceFinder.Get<ILightService>();
        }

        protected virtual void Start()
        {
            _initIntensity = Light.intensity;
            _initColor = Light.color;
        }

        public void Pool(Vector3 position, Quaternion rotation, float size = 1)
        {
        }

        public virtual void DoReset()
        {
            Light.color = _initColor;
            Light.intensity = _initIntensity;
        }

        public virtual void Wake()
        {
            if (Light == null || _initIntensity == 0)
                return;

            InterpolationHelper<Light2D, float>.Execute(new LightIntensityInterpolation(Light, Light.intensity, Light.intensity, _initIntensity, .3f));
        }

        public virtual void Sleep()
        {
            if (Light == null)
                return;

            InterpolationHelper<Light2D, float>.Execute(new LightIntensityInterpolation(Light, Light.intensity, Light.intensity, 0, .3f));
        }

        public void SetColor(CustomColor color, float intensity = 1)
        {
            SetColor(ColorUtils.GetColor(color), intensity);
        }

        public void SetColor(Color color, float intensity = 1)
        {
            Light.color = color;
            Light.intensity = intensity;
        }
    }
}
