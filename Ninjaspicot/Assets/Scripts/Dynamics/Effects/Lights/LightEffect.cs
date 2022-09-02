using UnityEngine;
using UnityEngine.Rendering.Universal;
using ZepLink.RiceNinja.Dynamics.Abstract;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Effects.Lights
{
    public abstract class LightEffect : Dynamic, IPoolable
    {
        public Light2D Light { get; protected set; }

        protected virtual void Awake()
        {
            Light = GetComponentInChildren<Light2D>();
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
            enabled = false;
        }

        public virtual void Wake()
        {
            enabled = true;
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
