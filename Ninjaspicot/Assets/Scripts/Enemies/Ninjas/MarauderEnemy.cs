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
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    Attacking = true;
                    Stickiness.NinjaDir = (Dir)1 - (int)Stickiness.NinjaDir;
                    _remainingTime = _walkTime;
                    Stickiness.StartWalking();
                    _fieldOfView.SetActive(false);
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

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_readyToStop)
            return;

        Stickiness.StopWalking(true);
        Attacking = false;
        _remainingTime = _searchTime;
        _marauderMode = MarauderMode.Searching;
        _fieldOfView.SetActive(true);
        _readyToStop = false;
    }

    public override bool NeedsToWalk()
    {
        return Attacking;
    }
}
