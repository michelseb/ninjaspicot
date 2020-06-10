using System.Collections;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _imgInside;
    [SerializeField] public SpriteMask Mask;
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
    private PortalManager _portalManager;
    private Coroutine _connect;

    private const int TRANSFER_SPEED = 3; //Seconds needed to go between 2 portals

    protected void Awake()
    {
        Mask.enabled = false;
        _cameraBehaviour = CameraBehaviour.Instance;
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
        if (_portalMovement != null) StopCoroutine(_portalMovement);
        _portalMovement = StartCoroutine(StartPortal());

        if (_portalManager.Connecting)
            return;

        Hero = collision.GetComponent<Hero>() ?? collision.GetComponentInParent<Hero>();

        if (_connect == null)
        {
            _connect = StartCoroutine(Connect());
        }
    }

    private IEnumerator Connect()
    {
        _portalManager.LaunchConnection(this);

        while (_portalManager.Connection != null)
            yield return null;

        if (Other == null || Exit)
        {
            Reinit(false);
            yield break;
        }

        Other.Hero = Hero;
        TeleportedRenderer = Hero.GetComponent<SpriteRenderer>();
        Other.TeleportedLayer = TeleportedLayer;
        SpriteMaskInteraction = TeleportedRenderer.maskInteraction;
        Other.SpriteMaskInteraction = SpriteMaskInteraction;

        Mask.enabled = true;
        Other.Mask.enabled = true;

        TeleportedRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        foreach (var renderer in Hero.GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        }


        TeleportedLayer = Hero.gameObject.layer;
        Hero.gameObject.layer = LayerMask.NameToLayer("Teleported");
        Hero.SetCapeActivation(false);

        StartCoroutine(Teleport());
        _connect = null;
    }


    public void Reinit(bool success)
    {
        if (_portalMovement != null) StopCoroutine(_portalMovement);
        _portalMovement = StartCoroutine(StopPortal());

        if (success && Entrance)
        {
            _portalManager.ClosePreviousZone(Id);
            Entrance = false;
            return;
        }


        Mask.enabled = false;

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

    private IEnumerator Teleport()
    {
        var transferredPoint = transform.InverseTransformPoint(Hero.transform.position);
        var transferredVelocity = -transform.InverseTransformDirection(Hero.Stickiness.Rigidbody.velocity);

        Hero.Stickiness.Rigidbody.velocity = Vector2.zero;
        Hero.Stickiness.Rigidbody.isKinematic = true;

        yield return new WaitForSeconds(1);

        _cameraBehaviour.SetCenterMode(Other.transform, TRANSFER_SPEED);

        yield return new WaitForSeconds(TRANSFER_SPEED);
        //var cam = _cameraBehaviour.Transform;
        //var pos0 = cam.position;
        //var pos1 = new Vector3(Other.transform.position.x, Other.transform.position.y, cam.position.z);
        //var dist = Vector3.Distance(pos0, pos1);
        //var startTime = Time.time;

        //float journey = 0;

        //while (journey < 1)
        //{
        //    float distCovered = (Time.time - startTime) * dist / TRANSFER_SPEED;
        //    journey = distCovered / dist;
        //    cam.position = Vector3.Lerp(pos0, pos1, journey);
        //    yield return null;
        //}

        _cameraBehaviour.SetFollowMode(Hero.transform);
        Hero.Stickiness.Rigidbody.position = Other.transform.TransformPoint(transferredPoint);

        yield return new WaitForSeconds(1);

        Reinit(true);
        Other.Reinit(true);
        Hero.Stickiness.Rigidbody.isKinematic = false;
        Hero.Stickiness.Rigidbody.velocity = Other.transform.TransformDirection(transferredVelocity);
        Hero.SetCapeActivation(true);
    }

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
