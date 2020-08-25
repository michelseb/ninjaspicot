
using UnityEngine;

public class EnemyJumper : Jumper, IRaycastable
{
    [SerializeField] private float _initCoolDown;

    private float _coolDown;

    protected int _id;
    public int Id { get { if (_id == 0) _id = gameObject.GetInstanceID(); return _id; } }

    protected override void Start()
    {
        base.Start();
        _coolDown = _initCoolDown;
    }


    protected virtual void Update()
    {
        if (_coolDown > 0)
        {
            _coolDown -= Time.deltaTime;

        }
        else
        {
            var heroPos = Hero.Instance.transform.position;
            //var offset = (heroPos - _transform.position).normalized * 4;
            var hit = Utils.RayCast(_transform.position, heroPos - _transform.position, 80f, Id, false);//Physics2D.Linecast(_transform.position + offset, Hero.Instance.transform.position);
            if (hit && hit.collider.CompareTag("hero"))
            {
                Jump(CalculateVelocity(Hero.Instance.transform.position, _transform.position, 2));
                _coolDown = _initCoolDown;
            }

        }
    }


    protected virtual Vector3 CalculateVelocity(Vector3 target, Vector3 origin, float duration)
    {
        var pathVector = target - origin;
        pathVector.z = -5;

        float distX = pathVector.magnitude;
        float distY = pathVector.y;

        float Vx = distX / duration;
        float Vy = distY / duration + Mathf.Abs(Physics.gravity.y) * duration;

        var result = pathVector.normalized;
        result *= Vx;
        result.y = Vy;

        return result;
    }
}
