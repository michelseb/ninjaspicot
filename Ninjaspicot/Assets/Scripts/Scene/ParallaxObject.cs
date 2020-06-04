using UnityEngine;

public class ParallaxObject : MonoBehaviour
{
    [SerializeField] private int _depth;
    [SerializeField] private bool _randomDepth;
    [SerializeField] private bool _initSettings;
    public int Depth => _depth > 0 ? _depth : ParallaxManager.MIN_DEPTH;

    private SpriteRenderer _renderer;
    private ParallaxManager _parallaxManager;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _parallaxManager = ParallaxManager.Instance;
    }

    private void Start()
    {
        if (_randomDepth)
        {
            _depth = Random.Range(ParallaxManager.MIN_DEPTH, ParallaxManager.MAX_DEPTH + 1);
        }

        if (_initSettings)
        {
            var colorFactor = (float)1 / Depth;
            _renderer.color = new Color(_renderer.color.r * colorFactor, _renderer.color.g * colorFactor, _renderer.color.b * colorFactor);
            transform.Translate(0, 0, 5 * Mathf.Log(Depth + 1), Space.World);
            transform.localScale = transform.localScale / Depth;
        }

        _renderer.sortingOrder = ParallaxManager.MAX_DEPTH - Depth;

        _parallaxManager.AddObject(this);
    }
}
