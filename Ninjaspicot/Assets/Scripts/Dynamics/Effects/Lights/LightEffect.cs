using UnityEngine;
using UnityEngine.Rendering.Universal;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Effects.Lights
{
    public abstract class LightEffect : Dynamic
    {
        protected Light2D _light;

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
