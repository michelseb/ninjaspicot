﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class TurretBase : MonoBehaviour, IActivable, IRaycastable
{
    protected int _id;
    public int Id { get { if (_id == 0) _id = gameObject.GetInstanceID(); return _id; } }
    public enum Mode { Scan, Aim, Wait };

    [SerializeField] protected float _viewAngle;
    [SerializeField] protected float _rotationSpeed;
    [SerializeField] protected bool _clockWise;
    [SerializeField] protected float _initRotation;

    public bool Loaded { get; protected set; }
    public Mode TurretMode { get; protected set; }
    public bool Active { get; protected set; }

    protected Aim _aim;
    protected Image _image;
    protected Transform _target;
    protected Coroutine _search;

    protected virtual void Awake()
    {
        _aim = GetComponentInChildren<Aim>();
        _aim.CurrentTarget = "hero";
        _image = GetComponent<Image>();
    }

    protected virtual void Start()
    {
        TurretMode = Mode.Scan;
        transform.Rotate(0, 0, _initRotation);
        Loaded = true;
        Active = true;
    }

    protected virtual void Update()
    {
        _image.color = Active ? ColorUtils.Red : ColorUtils.White;

        if (!Active)
            return;


        switch (TurretMode)
        {
            case Mode.Aim:

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, _target.transform.position - transform.position), .05f);

                if (_target == null || _aim.TargetAimedAt(_target, Id))
                {
                    StartWait();
                }
                break;

            case Mode.Scan:

                var dir = _clockWise ? 1 : -1;

                transform.Rotate(0, 0, _rotationSpeed * Time.deltaTime * dir);

                if (dir * (transform.rotation.eulerAngles.z - _initRotation) > _viewAngle)
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

    protected virtual IEnumerator Wait()
    {
        yield return new WaitForSeconds(2);

        if (TurretMode != Mode.Aim)
        {
            TurretMode = Mode.Scan;
        }
    }

    public void Activate()
    {
        Active = true;
    }

    public void Deactivate()
    {
        Active = false;
    }
}