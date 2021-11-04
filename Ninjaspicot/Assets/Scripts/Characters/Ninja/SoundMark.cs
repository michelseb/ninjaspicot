using System.Collections;
using UnityEngine;

public class SoundMark : Dynamic, IPoolable
{
    private ParticleSystem _particleSystem;

    public PoolableType PoolableType => PoolableType.None;
    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    public void Pool(Vector3 position, Quaternion rotation, float size)
    {
        Transform.localScale = Vector3.one * size;
        Transform.position = new Vector3(position.x, position.y, -5);
        Transform.rotation = rotation;
    }

    public void Sleep()
    {
        if (!gameObject.activeInHierarchy)
            return;

        _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        StartCoroutine(FadeAway());
    }

    public void Wake()
    {
        gameObject.SetActive(true);
    }

    protected virtual IEnumerator FadeAway()
    {
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);
    }

    public void DoReset()
    {
        Sleep();
    }
}
