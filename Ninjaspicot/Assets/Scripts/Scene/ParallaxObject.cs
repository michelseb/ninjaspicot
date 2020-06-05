using UnityEngine;

public class ParallaxObject : MonoBehaviour
{
    [SerializeField] private int _depth;
    [SerializeField] private bool _randomDepth;
    [SerializeField] private bool _initSettings;
    public int Depth => _depth > 0 ? _depth : ParallaxManager.MIN_DEPTH;
    public float ParallaxFactor { get; internal set; }

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
            var scaleFactor = ParallaxManager.SCALE_AMPLITUDE - (ParallaxManager.SCALE_AMPLITUDE * ((float)Depth / ParallaxManager.MAX_DEPTH));
            ParallaxFactor = (float)Depth / ParallaxManager.MAX_DEPTH;
            _renderer.color = new Color(_renderer.color.r * scaleFactor, _renderer.color.g * scaleFactor, _renderer.color.b * scaleFactor, scaleFactor);

            var factor = 2 * Mathf.Log(Depth + 1);
            transform.Translate(0, factor * factor, 0, Space.World);
            transform.localScale = transform.localScale * scaleFactor;
        }

        _renderer.sortingOrder = ParallaxManager.MAX_DEPTH - Depth;

        _parallaxManager.AddObject(this);
    }

}
