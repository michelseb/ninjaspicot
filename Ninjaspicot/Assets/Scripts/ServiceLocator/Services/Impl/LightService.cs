﻿using ZepLink.RiceNinja.Dynamics.Effects.Lights;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class LightService : InstantiatorService<LightEffect>, ILightService
    {
        protected override string ModelPath => "Lights";

        public void SetLightsActivation(bool active)
        {
            foreach (var light in Collection)
            {
                light.enabled = active;
            }
        }
    }
}