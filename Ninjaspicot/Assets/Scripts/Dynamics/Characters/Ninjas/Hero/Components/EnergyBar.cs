using System;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Abstract;
using ZepLink.RiceNinja.Manageables.Interfaces;

namespace ZepLink.RiceNinja.Dynamics.Characters.Hero.Components
{
    public class EnergyBar : Dynamic, IComponent
    {
        private enum EnergyState
        {
            Undefined = 0,
            Used = 1,
            Using = 2,
            Gaining = 3,
            Maxed = 4
        }

        public event Action OnEnergyConsumed;

        private float _value;
        private float _maxValue;
        private EnergyState _state;

        private float _coolDown = 1f;
        private float _remainingCoolDown;
        private bool _recovering;

        private const float STEP_AMOUNT = 1f;
        public bool HasEnergy => _value > 0;

        protected virtual void Start()
        {
            _remainingCoolDown = _coolDown;
            _state = EnergyState.Undefined;
        }

        //protected virtual void Update()
        //{
        //    if (!ApplyCoolDown())
        //    {
        //        _recovering = false;
        //        GainEnergy();
        //    }
        //}

        //private bool ApplyCoolDown()
        //{
        //    if ((_state == EnergyState.Used || _state == EnergyState.Using) && !_recovering)
        //    {
        //        _remainingCoolDown = _coolDown;
        //        _recovering = true;
        //    }

        //    if (!_recovering)
        //        return false;

        //    _remainingCoolDown -= Time.deltaTime;

        //    return _remainingCoolDown > 0;
        //}

        public bool ConsumeStep()
        {
            if (_value < STEP_AMOUNT || _state == EnergyState.Used)
                return false;

            SetConsumingState();

            _value = Mathf.Floor(_value - STEP_AMOUNT);

            if (_value <= 0)
            {
                SetConsumedState();
            }

            return true;
        }

        public void GainEnergy(float factor = 1)
        {
            if (factor <= 0 || _state == EnergyState.Maxed)
                return;

            _state = EnergyState.Gaining;

            _value = Mathf.Clamp(_value + Time.deltaTime * factor, 0, _maxValue);

            if (_value >= _maxValue)
            {
                _state = EnergyState.Maxed;
            }
        }

        public bool UseEnergy(float factor = 1)
        {
            if (factor <= 0 || _state == EnergyState.Used)
                return false;

            SetConsumingState();

            _value = Mathf.Clamp(_value - Time.deltaTime * factor, 0, _maxValue);

            if (_value <= 0)
            {
                SetConsumedState();
            }

            return true;
        }

        private void SetConsumingState()
        {
            _state = EnergyState.Using;
            _recovering = false;
        }

        private void SetConsumedState()
        {
            _state = EnergyState.Used;
            OnEnergyConsumed();
            RegainAll();
        }

        public void RegainAll()
        {
            _value = _maxValue;
            _state = EnergyState.Maxed;
        }

        public void SetMaxValue(float maxValue)
        {
            if (maxValue < 0)
                return;

            _maxValue = maxValue;

            _value = Mathf.Clamp(_value, 0, _maxValue);

            if (_value >= _maxValue)
            {
                _value = _maxValue;
                _state = EnergyState.Maxed;
            }
        }
    }
}