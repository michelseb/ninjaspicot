﻿using System.Collections;
using UnityEngine;

public abstract class Bonus : MonoBehaviour, IActivable, IWakeable
{
    [SerializeField] protected bool _respawn;
    [SerializeField] protected float _respawnTime;

    protected Collider2D[] _colliders;
    protected SpriteRenderer[] _renderers;
    protected Animator _animator;
    protected AudioSource _audioSource;
    protected AudioManager _audioManager;
    protected bool _active;

    private Zone _zone;
    public Zone Zone { get { if (Utils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

    protected virtual void Awake()
    {
        _colliders = GetComponentsInChildren<Collider2D>();
        _renderers = GetComponentsInChildren<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _audioManager = AudioManager.Instance;
    }

    protected virtual void Start()
    {
        _active = true;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("hero"))
            return;

        if (_respawn)
        {
            if (_active)
            {
                StartCoroutine(TemporaryDeactivation(_respawnTime));
            }
        }
        else
        {
            Deactivate();
            Destroy(gameObject, 1f);
        }
    }

    protected IEnumerator TemporaryDeactivation(float time)
    {
        Deactivate();
        yield return new WaitForSeconds(time);
        Activate();
    }

    public void Activate()
    {
        _active = true;
        _animator.enabled = true;

        foreach (var collider in _colliders)
        {
            collider.enabled = true;
        }

        foreach (var renderer in _renderers)
        {
            renderer.enabled = true;
        }
    }

    public void Deactivate()
    {
        _active = false;
        _animator.enabled = false;

        foreach (var collider in _colliders)
        {
            collider.enabled = false;
        }

        foreach (var renderer in _renderers)
        {
            renderer.enabled = false;
        }
    }

    public void Sleep()
    {
        _animator.SetTrigger("Sleep");
    }

    public void Wake()
    {
        _animator.SetTrigger("Wake");
    }
}
