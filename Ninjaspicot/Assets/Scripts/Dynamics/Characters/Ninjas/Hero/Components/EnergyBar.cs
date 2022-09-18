using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Abstract;

namespace ZepLink.RiceNinja.Dynamics.Characters.Hero.Components
{
    public class EnergyBar : Dynamic
    {
        private float _value;
        private float _maxValue;

        private const float STEP_AMOUNT = 1f;

        public bool ConsumeStep()
        {
            if (_value < STEP_AMOUNT)
                return false;

            _value = Mathf.Floor(_value - STEP_AMOUNT);

            return true;
        }

        public void GainEnergy(float factor = 1)
        {
            if (factor <= 0)
                return;

            _value = Mathf.Clamp(_value + Time.deltaTime * factor, 0, _maxValue);
        }

        public bool UseEnergy(float factor = 1)
        {
            if (factor <= 0 || _value <= 0)
                return false;

            _value = Mathf.Clamp(_value - Time.deltaTime * factor, 0, _maxValue);

            return true;
        }

        public void SetMaxValue(float maxValue)
        {
            _maxValue = maxValue;
        }
    }
}