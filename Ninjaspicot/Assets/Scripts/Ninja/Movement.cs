using System.Collections;
using UnityEngine;

public abstract class Movement : MonoBehaviour
{
    protected TimeManager _timeManager;
    protected CameraBehaviour _cameraBehaviour;
    protected DynamicEntity _dynamicEntity;

    protected float _strength;
    protected int _jumps;
    protected int _maxNumberOfJumpsAllowed;
    protected int _currentSpeed, _initialSpeed;

    protected virtual void Awake()
    {
        _cameraBehaviour = CameraBehaviour.Instance;
        _timeManager = TimeManager.Instance;
        _dynamicEntity = GetComponent<DynamicEntity>();
    }

    protected virtual void Start()
    {
        _strength = 100;
        _initialSpeed = 1000;
        _currentSpeed = _initialSpeed;
        _maxNumberOfJumpsAllowed = 4;
        _jumps = _maxNumberOfJumpsAllowed;
    }

    public virtual void Jump(Vector2 origin, Vector2 drag, float strength)
    {
        Vector2 forceToApply = origin - drag;
        LoseJump();
        _dynamicEntity.Rigidbody.velocity = new Vector2(0, 0);
        if (GetJumps() <= 0)
        {
            _timeManager.SetNormalTime();
        }

        _dynamicEntity.Rigidbody.AddForce(forceToApply.normalized * strength, ForceMode2D.Impulse);

    }

    public IEnumerator SpeedBoost(float time)
    {
        _currentSpeed *= 2;
        yield return new WaitForSeconds(time);
        _currentSpeed = _initialSpeed;
    }

    public void LoseJump()
    {
        _jumps--;
    }


    public int GetJumps()
    {
        return _jumps;
    }

    public int GetMaxJumps()
    {
        return _maxNumberOfJumpsAllowed;
    }

    public void SetMaxJumps(int amount)
    {
        _maxNumberOfJumpsAllowed = amount;
    }

    public void SetJumps(int amount)
    {
        _jumps = amount;
    }

    public void GainJumps(int amount)
    {
        _jumps += amount;

        if (_jumps > _maxNumberOfJumpsAllowed)
        {
            GainAllJumps();
        }
    }

    public void GainAllJumps()
    {
        _jumps = _maxNumberOfJumpsAllowed;
    }

    /*
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, .4f);
        Gizmos.DrawSphere(transform.TransformPoint(hinge.anchor), 1f);
    }*/

}
