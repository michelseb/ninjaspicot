using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Portal : MonoBehaviour, IActivable, IFocusable
{
    [SerializeField] private SpriteRenderer _imgInside;
    [SerializeField] private int _id;
    [SerializeField] private TextMeshProUGUI _title;

    public int Id => _id;
    public bool Exit { get; set; }
    public bool Entrance { get; set; }
    public bool IsSilent => true;
    public bool Taken { get; set; }
    public Portal Other { get; private set; }
    private Zone _zone;
    public Zone Zone { get { if (Utils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }
    public Transform Transform => transform;

    public bool Charge => false;

    public Hero Hero;
    public LayerMask TeleportedLayer;
    public Image TeleportedRenderer;
    public SpriteMaskInteraction SpriteMaskInteraction;
    private UICamera _uiCamera;
    private PortalManager _portalManager;
    private Coroutine _connect;
    private Coroutine _updateColor;
    private Animator _animator;
    private bool _titleVisible;

    private const float TITLE_DISTANCE = 50f;

    protected void Awake()
    {
        _uiCamera = UICamera.Instance;
        _portalManager = PortalManager.Instance;
        _animator = GetComponent<Animator>();
        _titleVisible = true;
    }

    protected void Start()
    {
        _portalManager.AddPortal(this);
    }

    protected void Update()
    {
        if (Time.frameCount % Utils.EXPENSIVE_FRAME_INTERVAL == 0)
        {
            var distFromHero = Vector3.Distance(Hero.Instance.Transform.position, transform.position);

            if (distFromHero < TITLE_DISTANCE && !_titleVisible)
            {
                if (_updateColor != null)
                {
                    StopCoroutine(_updateColor);
                }
                _updateColor = StartCoroutine(TitleAppear(_title, 2));
                _titleVisible = true;
            }
            else if (distFromHero >= TITLE_DISTANCE && _titleVisible)
            {
                if (_updateColor != null)
                {
                    StopCoroutine(_updateColor);
                }
                _updateColor = StartCoroutine(TitleDisappear(_title, 2));
                _titleVisible = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("hero"))
            return;

        if (_portalManager.Connecting)
            return;

        if (!_portalManager.ConnectionExists(this))
            return;

        Hero = collision.GetComponent<Hero>() ?? collision.GetComponentInParent<Hero>();

        if (Hero.Dead)
            return;

        Activate();
    }

    private IEnumerator TitleAppear(TextMeshProUGUI text, float duration)
    {
        float t = 0;
        var col = text.color;
        var endColor = new Color(col.r, col.g, col.b, 1);
        while (t < duration)
        {
            t += Time.deltaTime;
            text.color = Color.Lerp(col, endColor, t);
            yield return null;
        }
        _updateColor = null;
    }

    private IEnumerator TitleDisappear(TextMeshProUGUI text, float duration)
    {
        float t = 0;
        var col = text.color;
        var endColor = new Color(col.r, col.g, col.b, 0);
        while (t < duration)
        {
            t += Time.deltaTime;
            text.color = Color.Lerp(col, endColor, t);
            yield return null;
        }
        _updateColor = null;
    }

    private IEnumerator Connect()
    {
        _portalManager.StartPortalSound();

        yield return new WaitForSecondsRealtime(2);

        _portalManager.LaunchConnection(this);

        while (_portalManager.Connection != null)
            yield return null;

        if (Other == null || Exit)
        {
            Deactivate();
            yield break;
        }

        Other.Hero = Hero;
        TeleportedRenderer = Hero.GetComponent<Image>();
        Other.TeleportedLayer = TeleportedLayer;
        //SpriteMaskInteraction = TeleportedRenderer.maskInteraction;
        Other.SpriteMaskInteraction = SpriteMaskInteraction;

        //TeleportedRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;

        foreach (var renderer in Hero.GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        }

        TeleportedLayer = Hero.gameObject.layer;
        Hero.gameObject.layer = LayerMask.NameToLayer("Teleported");
        Hero.SetCapeActivation(false);

        _portalManager.StartCoroutine(_portalManager.Teleport(this, Other));
        _connect = null;
    }

    public void SetOtherPortal(Portal other)
    {
        Other = other;
    }

    public void Activate()
    {
        _animator.SetTrigger("Wake");

        Hero.DeactivateGrappling();
        Hero.Stickiness.Rigidbody.velocity = Vector2.zero;
        Hero.Stickiness.Rigidbody.isKinematic = true;
        Taken = true;

        if (_connect == null)
        {
            _uiCamera.CameraFade();
            Hero.StartFading();
            _connect = StartCoroutine(Connect());
        }
    }

    public void Deactivate()
    {
        _animator.SetTrigger("Sleep");

        if (Entrance)
        {
            Entrance = false;
            return;
        }

        if (TeleportedRenderer != null)
        {
            //TeleportedRenderer.maskInteraction = SpriteMaskInteraction;
            foreach (var renderer in Hero.GetComponentsInChildren<SpriteRenderer>())
            {
                renderer.maskInteraction = SpriteMaskInteraction;
            }
        }

        Hero.gameObject.layer = TeleportedLayer;
        Exit = false;
        Taken = false;
    }

    public void GoTo()
    {
    }
}
