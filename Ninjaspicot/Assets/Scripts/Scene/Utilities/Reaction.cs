using System.Collections;
using TMPro;
using UnityEngine;

public class Reaction : MonoBehaviour, IPoolable
{
    public PoolableType PoolableType => PoolableType.None;

    private Transform _transform;
    private TextMeshPro _textMesh;
    public Transform Transform { get { if (_transform == null) _transform = transform; return _transform; } }
    private Coroutine _deactivate;

    private void Awake()
    {
        _textMesh = GetComponent<TextMeshPro>();
    }

    public void Pool(Vector3 position, Quaternion rotation, float size = 1)
    {
        Transform.position = new Vector3(position.x, position.y, -5);
        Transform.rotation = rotation;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        var color = _textMesh.color;
        _textMesh.color = new Color(color.r, color.g, color.b, 1);
    }

    public void Deactivate()
    {
        if (Utils.IsNull(_deactivate))
        {
            _deactivate = StartCoroutine(DoDeactivate());
        }
    }

    private IEnumerator DoDeactivate()
    {
        var alpha = 1f;
        var color = _textMesh.color;
        while (alpha > 0)
        {
            alpha -= Time.deltaTime;
            _textMesh.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        _deactivate = null;
        gameObject.SetActive(false);
    }

    public void SetReaction(string reaction)
    {
        _textMesh.text = reaction;
    }
}
