using System.Collections;
using UnityEngine;

public class GrapplingGun : Dynamic
{
    [SerializeField] private float _cordSpeed;
    [SerializeField] private float _bodyAccelerationSpeed;
    [SerializeField] private float _bodyMaxSpeed;
    [SerializeField] private float _initJumpForce;
    [SerializeField] private Transform _shootOrigin;

    private LineRenderer _cord;
    private Hero _hero;
    private Coroutine _throwing;
    private Coroutine _pulling;
    private Rigidbody2D _rigidBody;
    private AudioManager _audioManager;
    private AudioSource _audioSource;
    private Audio _throwSound;
    private Audio _hitSound;


    private void Awake()
    {
        _cord = GetComponentInChildren<LineRenderer>();
        _hero = Hero.Instance;
        _rigidBody = _hero.Rigidbody;
        _cord.positionCount = 2;
        _audioManager = AudioManager.Instance;
        _audioSource = GetComponent<AudioSource>();
        _throwSound = _audioManager.FindAudioByName("GrapplingThrow");
        _hitSound = _audioManager.FindAudioByName("GrapplingHit");
    }


    public void StartThrow(Vector2 target)
    {
        if (_throwing != null)
            return;

        _audioManager.PlaySound(_audioSource, _throwSound);
        _throwing = StartCoroutine(DoThrow(target));
        Transform.rotation = Quaternion.Euler(0, 0, 90f) * Quaternion.LookRotation(Vector3.forward, target - Utils.ToVector2(Transform.position));
    }

    private IEnumerator DoThrow(Vector2 target)
    {
        //var direction = (target - _rigidBody.position).normalized;
        //if (Vector3.Dot(Vector3.up, direction) > 0)
        //{
        //    _hero.Stickiness.Detach();
        //    _rigidBody.AddForce(direction * _initJumpForce, ForceMode2D.Impulse);
        //}

        _cord.SetPosition(0, _shootOrigin.position);
        _cord.SetPosition(1, _shootOrigin.position);

        var cordCurrentPosition = _cord.GetPosition(1);
        while (Vector3.Distance(cordCurrentPosition, target) > 1f)
        {
            var nextPos = Vector3.MoveTowards(cordCurrentPosition, target, Time.deltaTime * _cordSpeed);
            _cord.SetPosition(0, _shootOrigin.position);
            _cord.SetPosition(1, nextPos);
            cordCurrentPosition = nextPos;

            yield return null;
        }

        _cord.SetPosition(1, target);
        StartPull(target);

        _throwing = null;
    }

    public void StartPull(Vector2 target)
    {
        if (_pulling != null)
            return;

        _audioManager.PlaySound(_audioSource, _hitSound);
        _pulling = StartCoroutine(DoPull(target));
    }

    private IEnumerator DoPull(Vector2 target)
    {
        var stickiness = _hero.Stickiness;
        var jumper = _hero.Jumper;
        float currentSpeed = 0f;

        var direction = (target - _rigidBody.position).normalized;
        _hero.DeactivateCollider();
        stickiness.Detach();

        _rigidBody.velocity = Vector2.zero;
        _rigidBody.gravityScale = 0;

        while (Vector3.Dot(direction, target - _rigidBody.position) > 0.5f && !_hero.Stickiness.Attached)
        {
            if (currentSpeed < _bodyMaxSpeed)
            {
                currentSpeed += _bodyAccelerationSpeed * Time.deltaTime;
            }
            else
            {
                currentSpeed = _bodyMaxSpeed;
            }

            _rigidBody.velocity = direction * currentSpeed;

            if (Vector3.Distance(_rigidBody.position, target) < 5)
            {
                _hero.ActivateCollider();
            }

            //rb.MovePosition(rb.position + direction * currentSpeed);
            _cord.SetPosition(0, _shootOrigin.position);
            Transform.rotation = Quaternion.Euler(0, 0, 90f) * Quaternion.LookRotation(Vector3.forward, target - Utils.ToVector2(Transform.position));
            yield return null;
        }

        jumper.ReinitGravity();
        _pulling = null;
        gameObject.SetActive(false);
    }
}
