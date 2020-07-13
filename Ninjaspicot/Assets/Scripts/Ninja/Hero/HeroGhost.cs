using UnityEngine;

public class HeroGhost : MonoBehaviour, IPoolable
{
    [SerializeField] private float _disappearingSpeed;
    [SerializeField] private float _initAlpha;
    [SerializeField] private Color _endColor;

    private SpriteRenderer _renderer;
    private Transform _transform;
    public PoolableType PoolableType => PoolableType.None;

    private void Awake()
    {
        _transform = transform;
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        var color = _renderer.color;

        if (color.a <= .01f)
        {
            Deactivate();
        }
        else
        {
            _renderer.color = Color.Lerp(color, _endColor, Time.deltaTime * _disappearingSpeed);
        }
    }


    public void Pool(Vector3 position, Quaternion rotation)
    {
        _transform.position = position;
        _transform.rotation = rotation;
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        var color = Hero.Instance.Renderer.color;
        _renderer.color = new Color(color.r, color.g, color.b, _initAlpha);
    }
}
