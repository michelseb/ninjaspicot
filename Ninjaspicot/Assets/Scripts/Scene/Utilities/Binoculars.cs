using UnityEngine;

public class Binoculars : CameraCatcher
{
    [SerializeField] private Transform _zoomCenter;

    private TriggerExit _trigger;
    private SpriteRenderer _renderer;
    private Color _initialColor;
    public override Transform ZoomCenter => _zoomCenter;

    protected override void Awake()
    {
        base.Awake();
        _trigger = GetComponentInChildren<TriggerExit>();
        _trigger.enabled = false;
        _renderer = GetComponent<SpriteRenderer>();
        _initialColor = _renderer.color;
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("hero") || _trigger.enabled)
            return;

        Activate();
        _trigger.SetActive(true);
    }

    public override void Activate()
    {
        base.Activate();
        _renderer.color = ColorUtils.Blue;
    }

    public override void Deactivate()
    {
        base.Deactivate();
        _renderer.color = _initialColor;
    }

    protected override void OnTriggerEnter2D(Collider2D collision) { }
    protected override void OnTriggerExit2D(Collider2D collision) { }
}
