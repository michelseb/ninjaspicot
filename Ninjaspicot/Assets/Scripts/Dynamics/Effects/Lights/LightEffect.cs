using UnityEngine;
using UnityEngine.Rendering.Universal;
using ZepLink.RiceNinja.Dynamics.Abstract;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Effects.Lights
{
    public abstract class LightEffect : Dynamic, IPoolable
    {
        protected Light2D _light;

        protected virtual void Awake()
        {
            _light = GetComponentInChildren<Light2D>();
        }

        public void Pool(Vector3 position, Quaternion rotation, float size = 1)
        {
        }

        public virtual void DoReset()
        {
            Sleep();
        }

        public virtual void Sleep()
        {
        }

        public virtual void Wake()
        {
        }

        public void SetColor(CustomColor color, float intensity = 1)
        {
            SetColor(ColorUtils.GetColor(color), intensity);
        }

        public void SetColor(Color color, float intensity = 1)
        {
            _light.color = color;
            _light.intensity = intensity;
        }
    }
}
