
using UnityEngine;

public class EnemyJumper : Jumper, IRaycastable
{
    [SerializeField] private float _initCoolDown;

    private float _coolDown;
    private bool _ready;

    protected int _id;
    public int Id { get { if (_id == 0) _id = gameObject.GetInstanceID(); return _id; } }

    protected override void Start()
    {
        base.Start();
        _coolDown = _initCoolDown;
    }


    protected virtual void Update()
    {
        if (!Active)
            return;

        if (_coolDown > 0)
        {
            _coolDown -= Time.deltaTime;
        }

        _ready = _coolDown <= 0;
    }

    public void JumpToPosition(Vector3 target)
    {
        if (!_ready)
            return;

        var hit = Utils.LineCast(_transform.position, target, new int[] { Id });

        if (hit && !hit.collider.CompareTag("hero"))
            return;

        CalculatedJump(CalculateVelocity(target));

        _ready = false;
        _coolDown = _initCoolDown;
    }


    protected virtual Vector3 CalculateVelocity(Vector3 target)
    {
        var pathVector = target - _transform.position;
        pathVector.z = 0;

        float distX = pathVector.magnitude;
        float distY = pathVector.y;

        float Vx = distX;
        float Vy = distY + Mathf.Abs(Physics.gravity.y) * 5;

        var result = pathVector.normalized;
        result *= Vx;
        result.y = Vy;

        return result;
    }
}
