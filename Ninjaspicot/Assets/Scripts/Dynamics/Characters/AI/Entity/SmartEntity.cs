using System;
using System.Collections.Generic;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Abstract;
using ZepLink.RiceNinja.Dynamics.Characters.AI.States;
using ZepLink.RiceNinja.Dynamics.Characters.Enemies;

namespace ZepLink.RiceNinja.Dynamics.Characters.AI.Entities
{
    public class SmartEntity : Dynamic
    {
        [SerializeField] private BaseState _initialState;

        public ISmart Smart { get; private set; }
        public Transform Target { get; private set; }

        private IDictionary<Type, Component> _components = new Dictionary<Type, Component>();

        public BaseState CurrentState { get; set; }

        private void Awake()
        {
            CurrentState = _initialState;
            Smart = base.GetComponent<ISmart>();
        }

        private void Update()
        {
            CurrentState.Execute(this);
        }

        public new T GetComponent<T>() where T : Component
        {
            var key = typeof(T);

            if (_components.ContainsKey(key))
                return _components[key] as T;

            if (TryGetComponent(out T component))
            {
                _components[key] = component;
            }
            else
            {
                component = GetComponentInChildren<T>() ?? GetComponentInParent<T>();
            }

            return component;
        }

        private new T GetComponentInChildren<T>() where T : Component
        {
            var key = typeof(T);

            if (_components.ContainsKey(key))
                return _components[key] as T;

            var component = base.GetComponentInChildren<T>();

            if (component != null)
            {
                _components[key] = component;
            }

            return component;
        }

        private new T GetComponentInParent<T>() where T : Component
        {
            var key = typeof(T);

            if (_components.ContainsKey(key))
                return _components[key] as T;

            var component = base.GetComponentInParent<T>();

            if (component != null)
            {
                _components[key] = component;
            }

            return component;
        }

        public void SetTarget(Transform target)
        {
            Target = target;
        }

        public void LooseTarget()
        {
            Target = null;
        }
    }
}