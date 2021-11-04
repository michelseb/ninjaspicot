using System.Collections;
using UnityEngine;

public class RobotLeg : Dynamic
{
    [SerializeField] private LegTarget _legTarget;
    [SerializeField] private float _liftTiming;

    private Vector2 _currentTarget;
    private const float REPLACEMENT_DISTANCE_THRESHOLD = 1.5f;
    private Coroutine _moveLeg;
    private SpiderBot _spiderBot;
    public bool Grounded => _moveLeg == null;

    private void Awake()
    {
        _spiderBot = GetComponentInParent<SpiderBot>();
    }

    private void Start()
    {
        _legTarget.CheckGround();
        ResetCurrentTarget();
    }
    private void Update()
    {
        UpdateTarget();
    }

    private void UpdateTarget()
    {
        if (_moveLeg != null || Mathf.Abs(_currentTarget.x - _legTarget.Transform.position.x) < REPLACEMENT_DISTANCE_THRESHOLD)
            return;

        LaunchMove();
    }

    public void ResetCurrentTarget()
    {
        _currentTarget = _legTarget.Transform.position + Vector3.right * _liftTiming;
    }

    public void LaunchMove()
    {
        if (_moveLeg != null) StopCoroutine(_moveLeg);
        _moveLeg = StartCoroutine(MoveLeg());
    }

    private IEnumerator MoveLeg()
    {
        var upPos = _currentTarget + Vector2.up * 2;

        while (Mathf.Abs(upPos.y - Transform.position.y) > .5f)
        {
            Transform.position = Vector3.MoveTowards(Transform.position, upPos, Time.deltaTime * _spiderBot.LegsSpeed);
            yield return null;
        }

        _currentTarget = _legTarget.Transform.position + _spiderBot.LegsStabilizationFactor * Transform.right * Mathf.Sign(_spiderBot.Speed);

        while (Vector2.Distance(_currentTarget, Transform.position) > .5f)
        {
            Transform.position = Vector3.MoveTowards(Transform.position, _currentTarget, Time.deltaTime * _spiderBot.LegsSpeed);
            yield return null;
        }

        _moveLeg = null;

    }
}
