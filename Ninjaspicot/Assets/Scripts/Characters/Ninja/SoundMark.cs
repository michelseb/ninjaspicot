using System.Collections;
using UnityEngine;

public class SoundMark : MonoBehaviour, IPoolable
{
    private Transform _transform;
    public Transform Transform { get { if (Utils.IsNull(_transform)) _transform = transform; return _transform; } }

    private ParticleSystem _particleSystem;

    public PoolableType PoolableType => PoolableType.None;

    private void Awake()
    {
        _transform = transform;
        _particleSystem = GetComponent<ParticleSystem>();
    }

    public void Pool(Vector3 position, Quaternion rotation, float size)
    {
        _transform.localScale = Vector3.one * size;
        _transform.position = new Vector3(position.x, position.y, -5);
        _transform.rotation = rotation;
    }

    public void Deactivate()
    {
        _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        StartCoroutine(FadeAway());
    }

    public void Activate()
    {
        //var main = _particleSystem.main;
        //var col = main.startColor.color;
        //col.a = 1;
        //main.startColor = col;
        gameObject.SetActive(true);
    }

    protected virtual IEnumerator FadeAway()
    {
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);
    }
}
