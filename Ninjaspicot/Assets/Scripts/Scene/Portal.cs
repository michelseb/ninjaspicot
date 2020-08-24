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
    private Coroutine _portalMovement;
    private CameraBehaviour _cameraBehaviour;
    private UICamera _uiCamera;
    private PortalManager _portalManager;
    private Coroutine _connect;

    protected void Awake()
    {
        _cameraBehaviour = CameraBehaviour.Instance;
        _uiCamera = UICamera.Instance;
        _portalManager = PortalManager.Instance;
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

        if (_portalMovement != null) StopCoroutine(_portalMovement);
        _portalMovement = StartCoroutine(StartPortal());

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
        if (_portalMovement != null) StopCoroutine(_portalMovement);
        _portalMovement = StartCoroutine(StopPortal());

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

    //private IEnumerator Teleport()
    //{
    //    Hero.Stickiness.Rigidbody.position = Other.transform.position - Other.transform.right * 4;
    //    _cameraBehaviour.Teleport(Hero.Stickiness.Rigidbody.position);
    //    _uiCamera.CameraAppear();

    //    yield return new WaitForSeconds(1);

    //    Reinit();
    //    Other.Reinit();
    //    Hero.Stickiness.Rigidbody.isKinematic = false;
    //    Hero.Stickiness.Rigidbody.velocity = Other.transform.right * PortalManager.EJECT_SPEED;
    //    Hero.SetCapeActivation(true);
    //    _portalManager.TerminateConnection();
    //}

    private IEnumerator StartPortal()
    {
        while (_rotationSpeed < 10)
        {
            _rotationSpeed += 100 * Time.deltaTime;
            _imgInside.color = new Color(_imgInside.color.r, _imgInside.color.g, _imgInside.color.b, Mathf.Max(_imgInside.color.a, _rotationSpeed / 40));
            yield return null;
        }
    }

    private IEnumerator StopPortal()
    {
        while (_rotationSpeed > 0)
        {
            _rotationSpeed -= 10 * Time.deltaTime;
            _imgInside.color = new Color(_imgInside.color.r, _imgInside.color.g, _imgInside.color.b, Mathf.Min(_imgInside.color.a, _rotationSpeed / 40));
            yield return null;
        }
    }

    public void SetOtherPortal(Portal other)
    {
        Other = other;
    }
}
