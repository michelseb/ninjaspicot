using System;
using System.Collections;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Interfaces;
using ZepLink.RiceNinja.Manageables.Audios;

namespace ZepLink.RiceNinja.Dynamics.Characters.Enemies.Machines.Turrets
{
    public class Sniper : Enemy, IActivable, IRaycastable
    {
        public enum Mode { Wonder, Aim };

        [SerializeField] protected float _initRotation;
        [SerializeField] protected float _wonderTime;
        [SerializeField] protected Transform _turretHead;

        public Mode TurretMode { get; protected set; }
        public bool Loaded { get; protected set; }
        public bool Seeing { get; set; }

        protected Vector3 _targetLocation;
        protected Coroutine _wait;

        protected override void Start()
        {
            base.Start();

            Loaded = true;
            //Activate();
            Deactivate();
        }

        protected virtual void Update()
        {

            Deactivate();
            if (!Active)
                return;

            switch (TurretMode)
            {
                case Mode.Aim:
                    Aim(_targetLocation);
                    break;


                case Mode.Wonder:
                    Wonder();
                    break;
            }

        }

        public virtual void Aim(Vector3 targetLocation)
        {
            _turretHead.rotation = Quaternion.RotateTowards(_turretHead.rotation, Quaternion.LookRotation(Vector3.forward, targetLocation - _turretHead.position), RotateSpeed);
        }

        protected virtual void Wonder()
        {
            StartAim(_targetLocation);
        }

        public void StartAim(Vector3 targetLocation)
        {
            _targetLocation = targetLocation;
            TurretMode = Mode.Aim;

            if (_wait != null)
            {
                StopCoroutine(_wait);
                _wait = null;
            }
        }

        public void StartWait()
        {
            TurretMode = Mode.Wonder;
            _wait = StartCoroutine(Wait());
        }

        protected virtual IEnumerator Wait()
        {
            yield return new WaitForSeconds(_wonderTime);

            if (TurretMode != Mode.Aim)
            {
                TurretMode = Mode.Wonder;
            }
        }

        public virtual void Activate(IActivator activator = default)
        {
            Active = true;
        }

        public virtual void Deactivate(IActivator activator = default)
        {
            Active = false;
        }

        public override void Wake()
        {
            Activate();
        }

        public override void Sleep()
        {
            Deactivate();
        }

        public override void Die(Transform killer = null, AudioFile sound = null, float volume = 1f)
        {
            throw new NotImplementedException();
        }

        public override IEnumerator Dying()
        {
            throw new NotImplementedException();
        }

        //protected override Action GetActionFromState(StateType stateType, StateType? nextState = null)
        //{
        //    return null;
        //}
    }
}