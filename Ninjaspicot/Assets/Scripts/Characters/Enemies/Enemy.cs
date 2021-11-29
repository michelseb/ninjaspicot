using System;
using System.Collections;
using UnityEngine;

public abstract class Enemy : Character, ISceneryWakeable, IResettable
{
    [SerializeField] protected StateType _initState;
    [SerializeField] protected float _rotateSpeed;
    [SerializeField] protected float _moveSpeed;

    public float MoveDirection { get; set; }

    public float Velocity => GetMovement();
    public float RotateSpeed => GetRotateSpeed();

    protected CharacterState _state;
    public CharacterState State
    {
        get
        {
            if (Utils.IsNull(_state))
            {
                SetState(_initState);
            }
            return _state;
        }
    }

    public bool Attacking { get; protected set; }
    public Transform TargetTransform { get; protected set; }
    public Vector3 TargetPosition { get; protected set; }

    public override Transform Transform
    {
        get 
        {
            if (Utils.IsNull(_transform)) _transform = Renderer?.transform ?? Image.transform; return _transform;
        }
    }

    public bool Active { get; protected set; }

    private Zone _zone;
    public Zone Zone { get { if (Utils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

    protected Animator _animator;

    protected Vector3 _resetPosition;
    protected Quaternion _resetRotation;

    protected override void Awake()
    {
        base.Awake();
        _animator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
    }

    protected override void Start()
    {
        base.Start();
        MoveDirection = Mathf.Sign(Velocity);
        _resetPosition = Transform.position;
        _resetRotation = Renderer?.transform.rotation ?? Image.transform.rotation;

        if (_startAwake)
        {
            Wake();
        }
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

        if (!Utils.IsNull(_animator)) _animator.SetTrigger("Sleep");

        _characterLight.Sleep();
        State.Sleep();
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

        if (!Utils.IsNull(_animator)) _animator.SetTrigger("Wake");
        SetState(_initState);

        _characterLight.Wake();
        State.Wake();
    }

    public void SetState(StateType stateType, StateType? nextState = null)
    {
        if (Utils.IsNull(_state))
        {
            _state = _poolManager.GetPoolable<CharacterState>(Transform.position, Quaternion.identity, 1f / Transform.lossyScale.magnitude, parent: Transform, defaultParent: false);
        }
        else if (_state.StateType == stateType)
        {
            return;
        }

        _state.SetState(stateType);

        // Launch related action
        GetActionFromState(stateType, nextState)?.Invoke();
    }

    public void SetNextState(StateType stateType)
    {
        State.SetNextState(stateType);
    }

    public bool IsState(StateType stateType)
    {
        return State.StateType == stateType;
    }

    public bool IsNextState(StateType stateType)
    {
        return State.NextState == stateType;
    }

    public override void Die(Transform killer = null, Audio sound = null, float volume = 1)
    {
        if (!Utils.IsNull(_state))
        {
            Destroy(_state.gameObject);
            _state = null;
        }

        if (!Utils.IsNull(Collider))
        {
            Collider.enabled = false;
        }

        Dead = true;
        Sleep();
    }

    public virtual void DoReset()
    {
        Attacking = false;
        SetState(_initState);
        Transform.position = _resetPosition;

        if (Renderer != null)
        {
            Renderer.transform.rotation = _resetRotation;
        }
        else
        {
            Image.transform.rotation = _resetRotation;
        }

        if (!Utils.IsNull(_animator)) _animator.Rebind();
        if (!Utils.IsNull(_animator)) _animator.Update(0);

        Dead = false;
        Wake();
    }

    protected virtual float GetRotateSpeed()
    {
        return _rotateSpeed * Time.deltaTime * GetRotationSpeedFactor(State.StateType);
    }

    protected virtual float GetMovement()
    {
        return _moveSpeed * Time.deltaTime * GetMovementSpeedFactor(State.StateType) * MoveDirection;
    }

    protected virtual float GetMovementSpeed()
    {
        return _moveSpeed * Time.deltaTime * GetMovementSpeedFactor(State.StateType);
    }

    protected virtual void Flip()
    {
        MoveDirection *= -1;
    }

    protected virtual float GetMovementSpeedFactor(StateType stateType)
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

    protected abstract Action GetActionFromState(StateType stateType, StateType? nextState = null);
}
