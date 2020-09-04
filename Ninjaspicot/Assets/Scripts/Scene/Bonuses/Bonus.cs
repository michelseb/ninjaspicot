using System.Collections;
using UnityEngine;

public abstract class Bonus : MonoBehaviour
{
    [SerializeField] protected bool _respawn;
    [SerializeField] protected float _respawnTime;

    protected Collider2D[] _colliders;
    protected SpriteRenderer[] _renderers;
    protected bool _active;
    
    protected virtual void Awake()
    {
        _colliders = GetComponentsInChildren<Collider2D>();
        _renderers = GetComponentsInChildren<SpriteRenderer>();
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
            SetActive(false);
            Destroy(gameObject, 1f);
        }
    }

    protected IEnumerator TemporaryDeactivation(float time)
    {
        SetActive(false);
        yield return new WaitForSeconds(time);
        SetActive(true);
    }

    protected void SetActive(bool active)
    {
        _active = active;

        foreach (var collider in _colliders)
        {
            collider.enabled = active;
        }

        foreach (var renderer in _renderers)
        {
            renderer.enabled = active;
        }
    }
}
