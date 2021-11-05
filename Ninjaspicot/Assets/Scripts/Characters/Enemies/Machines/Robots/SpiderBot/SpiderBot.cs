using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpiderBot : Robot
{
    [SerializeField] private Transform _body;
    [SerializeField] private float _legsSpeed;
    [SerializeField] private float _legsStabilizationFactor;


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

    public float Speed => _moveSpeed;
    public float LegsSpeed => _legsSpeed;
    public float LegsStabilizationFactor => _legsStabilizationFactor;
    private float _targetY;
    private Coroutine _pause;
    private float _moveDirection;

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

    private void CheckGround()
    {
        var hit = Utils.RayCast(_body.position, new Vector2(_moveDirection, -1), 8, includeTriggers: false);



        if (hit.collider != null)
        {
            _targetY = hit.point.y + 2f;
        }
        else if (_pause == null)
        {
            _pause = StartCoroutine(Pause(_moveSpeed));
        }
    }

    private void CheckWall()
    {
        var hit = Utils.RayCast(_body.position, Vector3.right * _moveSpeed, 5, includeTriggers: false);

        if (hit.collider != null)
        {
            Flip(_moveSpeed);
        }
    }

    private IEnumerator Pause(float speed)
    {
        _moveSpeed = 0;
        _robotLegs.ForEach(leg => leg.ResetCurrentTarget());

        yield return new WaitForSeconds(3);

        Flip(speed);
        _pause = null;
    }

    private void Flip(float speed)
    {
        _moveSpeed = -speed;
        _moveDirection = Mathf.Sign(_moveSpeed);
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
            _head.rotation = Quaternion.RotateTowards(_head.rotation, Quaternion.Euler(0f, 0f, 90f) * Quaternion.LookRotation(Vector3.forward, Transform.TransformDirection(direction)), GetRotateSpeed());
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
        _body.Translate(_body.right * _moveSpeed * Time.deltaTime);

        if (_moveSpeed != 0)
        {
            _moveDirection = Mathf.Sign(_moveSpeed);
        }

        var targetPos = new Vector3(_body.position.x, _targetY);

        if (!_robotLegs.Any(x => x.Grounded))
        {
            targetPos += Vector3.down;
        }

        _body.position = Vector3.MoveTowards(_body.position, targetPos, Time.deltaTime * 5);
        _head.rotation = Quaternion.RotateTowards(_head.rotation, Quaternion.Euler(0, 0, 90f) * Quaternion.LookRotation(Vector3.forward, _body.right * _moveDirection), GetRotateSpeed());

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
}
