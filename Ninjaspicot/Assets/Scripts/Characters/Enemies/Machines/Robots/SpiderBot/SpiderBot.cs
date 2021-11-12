using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpiderBot : Robot
{
    [SerializeField] private Transform _body;
    
    private float _legsSpeed;
    public int MovingLegsIndex = 0;
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
    private float _targetY;

    private List<RobotLeg> _robotLegs;

    protected override void Awake()
    {
        base.Awake();
        _robotLegs = GetComponentsInChildren<RobotLeg>().ToList();
    }

    protected override void Start()
    {
        base.Start();
        _targetY = _body.position.y;
    }

    protected override void Update()
    {
        base.Update();
        UpdateLegsSpeed();
    }

    private void UpdateLegsSpeed()
    {
        if (MoveSpeed == 0)
            return;

        _legsSpeed = Mathf.Abs(MoveSpeed) * 8;
    }

    private void CheckGround()
    {
        var hit = Utils.RayCast(_body.position, new Vector2(MoveDirection, -1), 8, includeTriggers: false);



        if (hit.collider != null)
        {
            _targetY = hit.point.y + 2f;
        }
        else
        {
            Flip();
        }
    }

    private void CheckWall()
    {
        var hit = Utils.RayCast(_body.position, Vector3.right * MoveSpeed, 5, includeTriggers: false);

        if (hit.collider != null)
        {
            Flip();
        }
    }

    #region Check
    protected override void Check()
    {
    }
    #endregion

    #region Chase
    protected override void Chase(Vector3 target)
    {
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

    #region Communicate
    protected override void Communicate()
    {
        _remainingCommunicationTime -= Time.deltaTime;

        if (_remainingCommunicationTime <= 0)
        {
            if (!Utils.IsNull(Zone) && Zone.DeathOccured)
            {
                Zone.ActivateAlarm();
            }
            else
            {
                SetState(State.NextState);
            }
        }
    }
    #endregion

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
        _body.Translate(_body.right * MoveSpeed * Time.deltaTime);

        var targetPos = new Vector3(_body.position.x, _targetY);

        if (!_robotLegs.Any(x => x.Grounded))
        {
            targetPos += Vector3.down;
        }

        //_body.position = Vector3.MoveTowards(_body.position, targetPos, Time.deltaTime * 5);
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
}
