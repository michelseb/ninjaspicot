using System.Collections;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _imgInside;
    [SerializeField] private int _id;

    public int Id => _id;
    public bool Exit { get; set; }
    public bool Entrance { get; set; }
    public Portal Other { get; private set; }

    public Hero Hero;
    private float _rotationSpeed;
    public LayerMask TeleportedLayer;
    public SpriteRenderer TeleportedRenderer;
    public SpriteMaskInteraction SpriteMaskInteraction;
    private UICamera _uiCamera;
    private PortalManager _portalManager;
    private Coroutine _connect;
    private Animator _animator;

    protected void Awake()
    {
        _uiCamera = UICamera.Instance;
        _portalManager = PortalManager.Instance;
        _animator = GetComponent<Animator>();
    }

    protected void Start()
    {
        _portalManager.AddPortal(this);
    }

    protected void FixedUpdate()
    {
        _imgInside.transform.Rotate(0, 0, _rotationSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("hero"))
            return;

        _animator.SetTrigger("Wake");

        if (_portalManager.Connecting)
            return;

        Hero = collision.GetComponent<Hero>() ?? collision.GetComponentInParent<Hero>();

        Hero.Stickiness.Rigidbody.velocity = Vector2.zero;
        Hero.Stickiness.Rigidbody.isKinematic = true;

        if (_connect == null)
        {
            _uiCamera.CameraFade();
            Hero.StartFading();
            _connect = StartCoroutine(Connect());
        }
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
            Reinit();
            yield break;
        }

        Other.Hero = Hero;
        TeleportedRenderer = Hero.GetComponent<SpriteRenderer>();
        Other.TeleportedLayer = TeleportedLayer;
        SpriteMaskInteraction = TeleportedRenderer.maskInteraction;
        Other.SpriteMaskInteraction = SpriteMaskInteraction;

        TeleportedRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;

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




    public void Reinit()
    {
        _animator.SetTrigger("Sleep");

        if (Entrance)
        {
            Entrance = false;
            return;
        }

        if (TeleportedRenderer != null)
        {
            TeleportedRenderer.maskInteraction = SpriteMaskInteraction;
            foreach (var renderer in Hero.GetComponentsInChildren<SpriteRenderer>())
            {
                renderer.maskInteraction = SpriteMaskInteraction;
            }
        }

        Hero.gameObject.layer = TeleportedLayer;
        Exit = false;
    }

    public void SetOtherPortal(Portal other)
    {
        Other = other;
    }
}
