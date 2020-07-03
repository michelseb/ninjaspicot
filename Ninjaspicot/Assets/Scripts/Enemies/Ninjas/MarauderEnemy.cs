using System.Collections;
using UnityEngine;

public class MarauderEnemy : EnemyNinja
{
    private enum MarauderMode
    {
        Searching,
        Moving
    }

    [SerializeField] private float _searchTime;
    [SerializeField] private float _walkTime;

    private FieldOfView _fieldOfView;
    private float _remainingTime;
    private MarauderMode _marauderMode;
    private bool _readyToStop;
    private Coroutine _slowDown;
    public override PoolableType PoolableType => PoolableType.Marauder;

    protected override void Awake()
    {
        base.Awake();
        _fieldOfView = GetComponentInChildren<FieldOfView>();
    }

    protected override void Update()
    {
        base.Update();

        if (Dead)
            return;

        _remainingTime -= Time.deltaTime;

        switch (_marauderMode)
        {
            case MarauderMode.Searching:

                if (_remainingTime <= 0)
                {
                    //transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    Attacking = true;
                    Stickiness.ReinitSpeed();
                    //Stickiness.NinjaDir = (Dir)1 - (int)Stickiness.NinjaDir;
                    _remainingTime = _walkTime;
                    Stickiness.StartWalking();
                    _fieldOfView.Deactivate();
                    _marauderMode = MarauderMode.Moving;
                }
                break;

            case MarauderMode.Moving:

                if (_remainingTime <= 0)
                {
                    _readyToStop = true;
                }
                break;
        }


        if (_readyToStop && _slowDown == null)
        {
            _slowDown = StartCoroutine(SlowDown());
        }
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (!_readyToStop)
    //        return;

    //    Stickiness.StopWalking(true);
    //    Attacking = false;
    //    _remainingTime = _searchTime;
    //    _marauderMode = MarauderMode.Searching;
    //    _fieldOfView.SetActive(true);
    //    _readyToStop = false;
    //}

    public override bool NeedsToWalk()
    {
        return Attacking;
    }

    private IEnumerator SlowDown()
    {
        var angle = Vector3.Angle(transform.up, Vector3.up);

        while (Stickiness.CurrentSpeed > 50)
        {
            var currAngle = Vector3.Angle(transform.up, Vector3.up);

            Stickiness.CurrentSpeed = Mathf.Lerp(Stickiness.CurrentSpeed, 0, (angle - currAngle) / angle * .05f);
            yield return null;
        }

        Stickiness.StopWalking(true);
        Attacking = false;
        _remainingTime = _searchTime;
        _marauderMode = MarauderMode.Searching;
        _fieldOfView.Activate();
        _readyToStop = false;
        _slowDown = null;
    }
}
