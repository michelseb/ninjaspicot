using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpiderBot : Robot
{
    [SerializeField] private Transform _body;

    private float _legsSpeed;
    public int MovingLegsIndex = 0;
    //private float _legsInitAverageY;
    //private float _legsAverageY;

    public override Transform Transform => _body;

    public override SpriteRenderer Renderer
    {
        get
        {
            if (Utils.IsNull(_renderer))
            {
                _renderer = _head.GetComponentInChildren<SpriteRenderer>();
            }

            return _renderer;
        }
    }

    protected Coroutine _lookAt;

    public float LegsSpeed => _legsSpeed;

    private List<RobotLeg> _robotLegs;

    protected override void Awake()
    {
        base.Awake();
        _robotLegs = GetComponentsInChildren<RobotLeg>().ToList();
    }

    //protected override void Start()
    //{
    //    base.Start();
    //    //_legsInitAverageY = GetLegsAverageY(_robotLegs);
    //}

    protected override void Update()
    {
        base.Update();
        UpdateLegsSpeed();
        //_legsAverageY = GetLegsAverageY(_robotLegs);
        //Transform.position += Vector3.up * (_legsAverageY - _legsInitAverageY);
        //Transform.rotation = Quaternion.Euler(0, 0, _legsAverageY - _legsInitAverageY);
    }

    private void UpdateLegsSpeed()
    {
        if (Velocity == 0)
            return;

        _legsSpeed = Mathf.Abs(Velocity) * 10;
    }

    private void CheckGround()
    {
        var hit = Utils.RayCast(_body.position, new Vector2(MoveDirection, -2 * Mathf.Abs(MoveDirection)), 16, includeTriggers: false);


        if (hit.collider == null)
        {
            Flip();
        }
    }

    private void CheckWall()
    {
        var hit = Utils.RayCast(_body.position, Vector3.right * Velocity, 15, includeTriggers: false);

        if (hit.collider != null)
        {
            Flip();
        }
    }

    #region Check
    protected override void Check()
    {
        var deltaX = TargetPosition.x - Transform.position.x;
        var direction = Mathf.Sign(deltaX);

        _head.rotation = Quaternion.RotateTowards(_head.rotation, Quaternion.Euler(0, 0, 90f) * Quaternion.LookRotation(Vector3.forward, TargetPosition - Transform.position), RotateSpeed);

        var wallNear = Utils.RayCast(_rigidbody.position, Vector3.right * direction, 6, Id);

        if (Mathf.Abs(deltaX) > 2 && !wallNear)
        {
            _body.Translate(Vector3.right * direction * GetMovementSpeed() * Time.deltaTime);
        }
        else if (Vector2.Dot(Utils.ToVector2(_head.right), Utils.ToVector2(TargetPosition - Transform.position).normalized) > .99f)
        {
            _hearingPerimeter.EraseSoundMark();
            SetState(StateType.Wonder, StateType.Return);
        }
    }
    #endregion

    #region Chase
    protected override void Chase(Vector3 target)
    {
        _head.rotation = Quaternion.RotateTowards(_head.rotation, Quaternion.Euler(0, 0, 90f) * Quaternion.LookRotation(Vector3.forward, Hero.Instance.Transform.position - Transform.position), RotateSpeed);

        var heroNotVisible = Utils.LineCast(Transform.position, target, Hero.Instance.Id);

        if (heroNotVisible)
        {
            Seeing = false;
            SetState(StateType.Wonder, StateType.Return);
        }
    }
    #endregion

    #region LookFor

    protected override void LookFor()
    {
        if (_lookAt == null)
        {
            _lookAt = StartCoroutine(LookAtRandom());
        }
        Guard();
    }

    protected virtual IEnumerator LookAtRandom()
    {
        float elapsedTime = 0;
        float delay = 2;
        var direction = Random.insideUnitCircle.normalized;

        while (elapsedTime < delay)
        {
            elapsedTime += Time.deltaTime;
            _head.rotation = Quaternion.RotateTowards(_head.rotation, Quaternion.Euler(0f, 0f, 90f) * Quaternion.LookRotation(Vector3.forward, Transform.TransformDirection(direction)), RotateSpeed);
            yield return null;
        }

        _lookAt = null;
    }
    #endregion

    //#region Communicate
    //protected override void Communicate()
    //{
    //    _remainingCommunicationTime -= Time.deltaTime;

    //    if (_remainingCommunicationTime <= 0)
    //    {
    //        if (!Utils.IsNull(Zone) && Zone.DeathOccured)
    //        {
    //            Zone.ActivateAlarm();
    //        }
    //        else
    //        {
    //            SetState(State.NextState);
    //        }
    //    }
    //}
    //#endregion

    #region Return
    protected override void Return()
    {
    }
    #endregion

    #region Guard
    protected override void Guard() { }
    #endregion

    #region Guard
    protected override void Patrol()
    {
        CheckGround();
        CheckWall();
        _body.Translate(_body.right * Velocity * Time.deltaTime);

        _head.rotation = Quaternion.RotateTowards(_head.rotation, Quaternion.Euler(0, 0, 90f) * Quaternion.LookRotation(Vector3.forward, _body.right * MoveDirection), RotateSpeed);
    }
    #endregion

    public override void Die(Transform killer = null, Audio sound = null, float volume = 1)
    {
        _hearingPerimeter.EraseSoundMark();
        base.Die(killer, sound, volume);
    }

    public override void DoReset()
    {
        Seeing = false;
        TargetTransform = null;

        if (_lookAt != null)
        {
            StopCoroutine(_lookAt);
            _lookAt = null;
        }

        base.DoReset();
    }

    public void ChangeMovingLegs(int index)
    {
        MovingLegsIndex = index;
    }

    //private float GetLegsAverageY(List<RobotLeg> legs)
    //{
    //    return legs.Average(leg => Transform.InverseTransformPoint(leg.Transform.position).y);
    //}

    protected override float GetMovementSpeedFactor(StateType stateType)
    {
        switch (stateType)
        {
            case StateType.Patrol:
            case StateType.Guard:
            case StateType.Return:
                return 1f;
            case StateType.Check:
                return 3f;
            case StateType.Chase:
            case StateType.LookFor:
                return 5f;
            default:
                return 0;
        }
    }
}
