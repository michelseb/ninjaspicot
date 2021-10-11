using System.Collections;
using UnityEngine;

public class SimulatedSoundEffect : MonoBehaviour, IPoolable
{
    private Transform _transform;
    public Transform Transform { get { if (Utils.IsNull(_transform)) _transform = transform; return _transform; } }

    private SpriteRenderer _renderer;

    public PoolableType PoolableType => PoolableType.None;
    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void Pool(Vector3 position, Quaternion rotation, float size)
    {
        Transform.localScale = Vector3.one * size;
        Transform.position = new Vector3(position.x, position.y, -5);
    }

    public void Sleep()
    {
        StartCoroutine(FadeAway());
    }

    public void Wake()
    {
        _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, 1);
        gameObject.SetActive(true);
    }

    protected virtual IEnumerator FadeAway()
    {
        Color col = _renderer.color;
        while (col.a > 0)
        {
            col = _renderer.color;
            col.a -= Time.deltaTime;
            _renderer.color = col;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
