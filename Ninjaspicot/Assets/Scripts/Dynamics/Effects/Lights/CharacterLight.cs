using UnityEngine;

namespace ZepLink.RiceNinja.Dynamics.Effects.Lights
{
    public class CharacterLight : LightEffect
    {
        protected virtual void LateUpdate()
        {
            Transform.rotation = Quaternion.identity;
        }
    }
}