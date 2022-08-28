using System.Collections;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Interfaces;
using ZepLink.RiceNinja.Manageables.Audios;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Characters.Enemies
{
    public abstract class Enemy : Character, ISceneryWakeable, IFocusable, IResettable
    {
        [SerializeField] protected StateType _initState;
        [SerializeField] protected Collider2D _castArea;
        [SerializeField] protected float _rotateSpeed;
        [SerializeField] protected float _moveSpeed;

        /// <summary>
        /// Move direction (between -1 and 1)
        /// </summary>
        public float MoveDirection { get; set; }

        /// <summary>
        /// Move magnitude
        /// </summary>
        public float MoveSpeed => GetMovementSpeed();
        public float RotateSpeed => GetRotateSpeed();

        /// <summary>
        /// Move speed * Move direction
        /// </summary>
        public float MoveVector => MoveDirection * MoveSpeed;

        //protected StateEffect _state;
        //public StateEffect State
        //{
        //    get
        //    {
        //        if (BaseUtils.IsNull(_state))
        //        {
        //            SetState(_initState);
        //        }
        //        return _state;
        //    }
        //}

        public bool Attacking { get; protected set; }
        public bool Active { get; protected set; }

        #region IFocusable
        public bool IsSilent => true;
        public bool Taken => false;
        #endregion

        protected Animator _animator;

        protected Vector3 _resetPosition;
        protected Quaternion _resetRotation;

        protected override void Awake()
        {
            base.Awake();
            _animator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
            Active = _startAwake;
        }

        protected override void Start()
        {
            base.Start();
            MoveDirection = Mathf.Sign(MoveSpeed * Transform.right.x);
            _resetPosition = Transform.position;
            _resetRotation = Renderer?.transform.rotation ?? Image.transform.rotation;
        }

        public override IEnumerator Dying()
        {
            var col = Renderer?.color ?? Image.color;
            var alpha = col.a;

            while (alpha > 0)
            {
                alpha -= Time.deltaTime;
                if (Renderer != null)
                {
                    Renderer.color = new Color(Renderer.color.r, Renderer.color.g, Renderer.color.b, Renderer.color.a - Time.deltaTime);
                }
                else if (Image != null)
                {
                    Image.color = new Color(Image.color.r, Image.color.g, Image.color.b, Image.color.a - Time.deltaTime);
                }

                yield return null;
            }

            Die();
        }

        protected void SetAttacking(bool attacking)
        {
            Attacking = attacking;

            if (Renderer != null)
            {
                Renderer.color = Attacking ? ColorUtils.Red : ColorUtils.White;
            }
            else if (Image != null)
            {
                Image.color = Attacking ? ColorUtils.Red : ColorUtils.White;
            }
        }

        public virtual void Sleep()
        {
            Active = false;

            //if (Renderer != null)
            //{
            //    Renderer.enabled = false;
            //}
            //else if (Image != null)
            //{
            //    Image.enabled = false;
            //}

            //Collider.enabled = false;

            if (!BaseUtils.IsNull(_animator))
            {
                _animator.SetTrigger("Sleep");
            }

            //State.Sleep();
        }

        public virtual void Wake()
        {
            // This game is not called the walking dead!
            if (Dead)
                return;

            Active = true;

            //if (Renderer != null)
            //{
            //    Renderer.enabled = true;
            //}
            //else if (Image != null)
            //{
            //    Image.enabled = true;
            //}

            if (!BaseUtils.IsNull(_animator))
            {
                _animator.SetTrigger("Wake");
            }

            //SetState(_initState);
            //State.Wake();
        }

        //public void SetState(StateType stateType, StateType? nextState = null)
        //{
        //    if (BaseUtils.IsNull(_state))
        //    {
        //        _state = PoolHelper.Pool<StateEffect>(Transform.position, Quaternion.identity, 1f / Transform.lossyScale.magnitude);
        //    }
        //    else if (_state.StateType == stateType)
        //    {
        //        return;
        //    }

        //    _state.SetState(stateType);

        //    // Launch related action
        //    GetActionFromState(stateType, nextState)?.Invoke();
        //}

        //public void SetNextState(StateType stateType)
        //{
        //    State.SetNextState(stateType);
        //}

        //public bool IsState(StateType stateType)
        //{
        //    return State.StateType == stateType;
        //}

        //public bool IsNextState(StateType stateType)
        //{
        //    return State.NextState == stateType;
        //}

        public override void Die(Transform killer = null, AudioFile sound = null, float volume = 1)
        {
            //if (!BaseUtils.IsNull(_state))
            //{
            //    Destroy(_state.gameObject);
            //    _state = null;
            //}

            if (!BaseUtils.IsNull(_castArea))
            {
                _castArea.enabled = false;
            }

            if (!BaseUtils.IsNull(Collider))
            {
                Collider.enabled = false;
            }

            Dead = true;
            Sleep();
        }

        public virtual void DoReset()
        {
            Attacking = false;
            //SetState(_initState);
            Transform.position = _resetPosition;

            if (Renderer != null)
            {
                Renderer.transform.rotation = _resetRotation;
            }
            else
            {
                Image.transform.rotation = _resetRotation;
            }

            if (!BaseUtils.IsNull(_animator)) _animator.Rebind();
            if (!BaseUtils.IsNull(_animator)) _animator.Update(0);

            Dead = false;
            Wake();
        }

        protected virtual float GetRotateSpeed()
        {
            return _rotateSpeed * Time.deltaTime;// * GetRotationSpeedFactor(State.StateType);
        }

        protected virtual float GetMovementSpeed()
        {
            return _moveSpeed * Time.deltaTime;// * GetMovementSpeedFactor(State.StateType) * MoveDirection;
        }

        protected virtual void Flip()
        {
            MoveDirection *= -1;
        }

        protected float GetMovementSpeedFactor(StateType stateType)
        {
            switch (stateType)
            {
                case StateType.Patrol:
                case StateType.Guard:
                case StateType.Return:
                    return 1f;
                case StateType.Check:
                    return 1.5f;
                case StateType.Chase:
                case StateType.LookFor:
                    return 1.8f;
                default:
                    return 0;
            }
        }

        protected float GetRotationSpeedFactor(StateType stateType)
        {
            switch (stateType)
            {
                case StateType.Patrol:
                case StateType.Guard:
                case StateType.Return:
                    return 1f;
                case StateType.Check:
                    return 1.5f;
                case StateType.Chase:
                case StateType.LookFor:
                    return 2f;
                default:
                    return 0;
            }
        }

        //protected abstract Action GetActionFromState(StateType stateType, StateType? nextState = null);
    }
}