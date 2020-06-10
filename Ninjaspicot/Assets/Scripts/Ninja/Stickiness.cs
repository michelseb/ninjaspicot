using System.Collections;
using UnityEngine;

public enum Dir
{
    Left,
    Right
}

public class Stickiness : MonoBehaviour, IDynamic
{
    [SerializeField] private int _speed;
    private Rigidbody2D _rigidbody;

    public HingeJoint2D WallJoint { get; set; }
    public Obstacle CurrentAttachment { get; set; }
    public bool Attached { get; private set; }
    public bool Active { get; set; }
    public bool CanWalk { get; set; }

    public Rigidbody2D Rigidbody { get { if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody2D>(); return _rigidbody; } }

    private IDynamic _dynamicEntity;
    private ContactPoint2D _contactPoint;
    private ContactPoint2D _previousContactPoint;
    private Coroutine _walkOnWalls;
    private Dir _ninjaDir;

    private TouchManager _touchManager;
    private JumpManager _jumpManager;

    public void Awake()
    {
        WallJoint = GetComponent<HingeJoint2D>();
        _touchManager = TouchManager.Instance;
        _dynamicEntity = GetComponent<IDynamic>();
        _jumpManager = GetComponent<JumpManager>();
    }

    public void Start()
    {
        Active = true;
        CanWalk = true;
    }

    private void Update()
    {
        _ninjaDir = _touchManager.TouchArea == TouchArea.Left ? Dir.Left : Dir.Right;

        if (_touchManager.Touching)
        {
            if (_jumpManager.CanJump())
            {
                StopWalking();
            }
            else if (_walkOnWalls == null && Attached && CanWalk)
            {
                _walkOnWalls = StartCoroutine(WalkOnWalls(WallJoint));
            }
        }
    }

    private void FixedUpdate()
    {
        if (Attached && !_touchManager.Touching)
        {
            _dynamicEntity.Rigidbody.velocity = new Vector2(0, 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        SetContactPoint(GetContactPoint(collision.contacts, _previousContactPoint));
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        SetContactPoint(GetContactPoint(collision.contacts, _previousContactPoint));
    }


    public void Attach()
    {
        WallJoint.enabled = true;
        WallJoint.useMotor = false;
        WallJoint.anchor = transform.InverseTransformPoint(GetContactPosition());
        WallJoint.connectedAnchor = WallJoint.anchor;

        Attached = true;

        _dynamicEntity.Rigidbody.gravityScale = 0;
        _dynamicEntity.Rigidbody.velocity = Vector2.zero;
    }

    public void Detach()
    {
        CurrentAttachment?.LaunchQuickDeactivate();
        _dynamicEntity.Rigidbody.gravityScale = 1;
        WallJoint.enabled = false;
        CurrentAttachment = null;
        Attached = false;
    }

    public void ReactToObstacle(Obstacle obstacle)
    {
        if (!Active || obstacle == CurrentAttachment)
            return;

        _jumpManager.GainAllJumps();

        if (tag == "Dynamic")
        {
            Hero.Instance?.JumpManager.GainAllJumps();
        }

        if (obstacle.CompareTag("Wall"))
        {
            Detach();
            Attach();
        }
    }

    private ContactPoint2D GetContactPoint(ContactPoint2D[] contacts, ContactPoint2D previous) //WOOOOHOOO ça marche !!!!!
    {
        ContactPoint2D resultContact = new ContactPoint2D();
        float dist = 0;
        foreach (ContactPoint2D contact in contacts)
        {
            if (Vector3.Distance(previous.point, contact.point) > dist)
            {
                dist = Vector3.Distance(previous.point, contact.point);
                resultContact = contact;
            }
        }

        return resultContact;
    }

    public void SetContactPoint(ContactPoint2D point)
    {
        _previousContactPoint = _contactPoint;
        _contactPoint = point;
    }

    public Vector3 GetContactPosition(bool local = false)
    {
        return local ? transform.InverseTransformPoint(_contactPoint.point) : new Vector3(_contactPoint.point.x, _contactPoint.point.y);
    }

    public void StopWalking()
    {
        if (_walkOnWalls != null)
        {
            StopCoroutine(_walkOnWalls);
        }
        Hero.Instance.Renderer.color = Color.white;
        WallJoint.useMotor = false;
        _walkOnWalls = null;
    }

    public IEnumerator WalkOnWalls(HingeJoint2D hinge)
    {
        if (hinge == null)
            yield break;

        Hero.Instance.Renderer.color = Color.green;

        var jointMotor = hinge.motor;
        WallJoint.useMotor = true;

        while (Input.GetButton("Fire1"))
        {
            jointMotor.motorSpeed = _ninjaDir == Dir.Left ? -_speed : _speed;
            hinge.motor = jointMotor;
            hinge.anchor = GetContactPosition(true);

            yield return null;
        }

        Hero.Instance.Renderer.color = Color.white;
        WallJoint.useMotor = false;
        _walkOnWalls = null;
    }

}
