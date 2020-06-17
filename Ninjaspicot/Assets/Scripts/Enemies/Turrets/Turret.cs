using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Turret : MonoBehaviour, IActivable, IRaycastable
{
    private int _id;
    public int Id { get { if (_id == 0) _id = gameObject.GetInstanceID(); return _id; } }
    public enum Mode { Scan, Aim, Wait };

    [SerializeField] private bool _autoShoot;
    [SerializeField] private float _viewAngle;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private bool _clockWise;
    [SerializeField] private float _strength;
    [SerializeField] private float _loadTime;

    public bool Loaded { get; private set; }
    public Mode TurretMode { get; private set; }
    public bool AutoShoot => _autoShoot;
    public bool Active { get; private set; }

    private float _initRotation;
    private float _loadProgress;

    private Aim _aim;
    private Image _image;
    private Transform _target;
    private Coroutine _search;

    private PoolManager _poolManager;

    private void Awake()
    {
        _aim = GetComponentInChildren<Aim>();
        _aim.CurrentTarget = "hero";
        _poolManager = PoolManager.Instance;
        _image = GetComponent<Image>();
    }

    private void Start()
    {
        TurretMode = Mode.Scan;
        _initRotation = transform.rotation.z;
        Loaded = true;
        Active = true;
    }

    private void Update()
    {
        _image.color = Active ? ColorUtils.Red : ColorUtils.White;

        if (!Active)
            return;

        //Loading weapon
        if (!Loaded)
        {
            if (_loadProgress >= _loadTime)
            {
                Loaded = true;
                _loadProgress = 0;
            }
            else
            {
                _loadProgress += Time.deltaTime;
            }
        }


        switch (TurretMode)
        {
            case Mode.Aim:

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, _target.transform.position - transform.position), .05f);

                if (_target != null && _aim.TargetAimedAt(_target, Id))
                {
                    if (Loaded && _aim.TargetCentered(transform, _target.tag, Id))
                    {
                        Shoot();
                    }
                }
                else
                {
                    StartWait();
                }
                break;


            case Mode.Scan:

                if (_autoShoot)
                {
                    if (Loaded)
                    {
                        Shoot();
                    }
                }

                var dir = _clockWise ? 1 : -1;

                transform.Rotate(0, 0, _rotationSpeed * Time.deltaTime * dir);

                if (Mathf.Abs(transform.rotation.z - _initRotation) > _viewAngle)
                {
                    _clockWise = !_clockWise;
                }

                break;


            case Mode.Wait:

                if (_target != null && _aim.TargetAimedAt(_target, Id))
                {
                    StartAim(_target);
                }
                break;
        }

    }


    public void StartAim(Transform target)
    {
        _target = target;
        TurretMode = Mode.Aim;

        if (_search != null)
        {
            StopCoroutine(_search);
            _search = null;
        }
    }

    public void StartWait()
    {
        TurretMode = Mode.Wait;
        _search = StartCoroutine(Wait());
    }

    private void Shoot()
    {
        var bullet = _poolManager.GetPoolable<Bullet>(transform.position, transform.rotation, PoolableType.Bullet);
        bullet.Speed = _strength;
        Loaded = false;
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(2);

        if (TurretMode != Mode.Aim)
        {
            TurretMode = Mode.Scan;
        }
    }

    public void SetActive(bool active)
    {
        Active = active;
    }
}