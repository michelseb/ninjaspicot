using System.Collections;
using UnityEngine;

public class Turret : MonoBehaviour, IActivable
{
    public enum Mode { Scan, Aim, Wait };

    [SerializeField] private bool _autoShoot;
    [SerializeField] private float _viewAngle;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private bool _clockWise;
    [SerializeField] private float _strength;
    [SerializeField] private float _loadTime;
    [SerializeField] private GameObject _bullet;
    
    public bool Loaded { get; private set; }
    public Mode TurretMode { get; private set; }
    public bool AutoShoot => _autoShoot;

    private bool _active;
    private float _initRotation;
    private float _loadProgress;
    
    private Transform _target;
    private Coroutine _search;

    private void Start()
    {
        TurretMode = Mode.Scan;
        _initRotation = transform.rotation.z;
        Loaded = true;
        _active = true;
    }

    private void Update()
    {
        if (!_active)
            return;

        transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);
        
        switch (TurretMode)
        {
            case Mode.Aim:

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(Vector3.up, _target.transform.position - _bullet.transform.position), .1f);
                if (Loaded)
                {
                    Shoot();
                    StartCoroutine(Load());
                    Loaded = false;
                }

                break;


            case Mode.Scan:

                if (_autoShoot)
                {
                    if (Loaded)
                    {
                        Shoot();
                        StartCoroutine(Load());
                        Loaded = false;
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

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(Vector3.up, _target.transform.position - _bullet.transform.position), .1f);

                break;
        }


        /*if (r.isVisible == false)
        {
            turretMode = Mode.Scan;
        }*/



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

    public void StartSearch()
    {
        if (TurretMode == Mode.Aim)
        {
            TurretMode = Mode.Wait;
            _search = StartCoroutine(Search());
        }
    }

    public void SelectMode(string evenement, Transform target = null)
    {
        switch (evenement)
        {
            case "aim":
                _target = target;
                if (TurretMode == Mode.Wait)
                {
                    StopCoroutine(_search);
                    TurretMode = Mode.Aim;
                }
                else if (TurretMode == Mode.Scan)
                {
                    TurretMode = Mode.Aim;
                }
                else
                {
                    if (TurretMode == Mode.Aim)
                    {
                        TurretMode = Mode.Wait;
                        _search = StartCoroutine(Search());
                    }
                }
                break;

            case "search":
                if (TurretMode == Mode.Aim)
                {
                    TurretMode = Mode.Wait;
                    _search = StartCoroutine(Search());
                }
                break;
        }
    }

    private void Shoot()
    {
        var direction = _bullet.transform.position - transform.position;
        GameObject b = Instantiate(_bullet, _bullet.transform.position + direction / 2, Quaternion.identity);
        b.transform.parent = null;
        b.AddComponent<Rigidbody2D>();
        b.AddComponent<Bullet>();
        b.GetComponent<Rigidbody2D>().gravityScale = 0;
        b.GetComponent<Rigidbody2D>().AddForce(direction * _strength, ForceMode2D.Impulse);
    }

    private IEnumerator Load()
    {
        while (_loadProgress < _loadTime)
        {
            _loadProgress += Time.deltaTime;
            yield return null;

        }

        Loaded = true;
        _loadProgress = 0;

    }

    private IEnumerator Search()
    {
        yield return new WaitForSeconds(2);

        if (TurretMode != Mode.Aim)
        {
            TurretMode = Mode.Scan;
        }

    }

    public void SetActive(bool active)
    {
        _active = active;
    }
}