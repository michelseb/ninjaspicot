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
    public Dir NinjaDir { get; set; }

    public Rigidbody2D Rigidbody { get { if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody2D>(); return _rigidbody; } }

    public bool DynamicActive => true;

    public PoolableType PoolableType => PoolableType.HeroCollider;

    private Ninja _ninja;
    private Transform _contactPoint;
    private Vector3 _previousContactPoint;
    private Coroutine _walkOnWalls;
    private Jumper _jumpManager;

    public void Awake()
    {
        if (Active)
            return;

        Active = true;
        WallJoint = GetComponent<HingeJoint2D>();
        _jumpManager = GetComponent<Jumper>();
        _ninja = GetComponent<Ninja>();
        _contactPoint = new GameObject("ContactPoint").transform;
        _contactPoint.position = transform.position;
        _contactPoint.SetParent(transform);
        _previousContactPoint = _contactPoint.position;
    }

    public void Start()
    {
        if (CanWalk)
            return;

        CanWalk = true;
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
        WallJoint.anchor = GetContactPosition();
        WallJoint.connectedAnchor = WallJoint.anchor;

        Attached = true;

        Rigidbody.gravityScale = 0;
        Rigidbody.velocity = Vector2.zero;
    }

    public void Detach()
    {
        CurrentAttachment?.LaunchQuickDeactivate();
        Rigidbody.gravityScale = 1;
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

    private ContactPoint2D GetContactPoint(ContactPoint2D[] contacts, Vector3 previousPos) //WOOOOHOOO ça marche !!!!!
    {
        ContactPoint2D resultContact = new ContactPoint2D();
        float dist = 0;
        foreach (ContactPoint2D contact in contacts)
        {
            if (Vector3.Distance(previousPos, contact.point) > dist)
            {
                dist = Vector3.Distance(previousPos, contact.point);
                resultContact = contact;
            }
        }

        return resultContact;
    }

    public void SetContactPoint(ContactPoint2D contact)
    {
        _previousContactPoint = _contactPoint.position;
        _contactPoint.position = contact.point;
    }

    public Vector3 GetContactPosition(bool local = true)
    {
        return local ? transform.InverseTransformPoint(_contactPoint.position) : _contactPoint.position;
    }

    public void SetContactPosition(Vector3 position, bool local = true)
    {
        if (local)
        {
            _contactPoint.localPosition = position;
        }
        else
        {
            _contactPoint.position = position;
        }
    }

    public void StopWalking()
    {
        if (_walkOnWalls != null)
        {
            StopCoroutine(_walkOnWalls);
        }

        WallJoint.useMotor = false;
        _walkOnWalls = null;
    }

    public void StartWalking()
    {
        if (_walkOnWalls != null)
            return;

        _walkOnWalls = StartCoroutine(WalkOnWalls(WallJoint));
    }

    public IEnumerator WalkOnWalls(HingeJoint2D hinge)
    {
        if (hinge == null)
            yield break;

        var jointMotor = hinge.motor;
        WallJoint.useMotor = true;

        while (_ninja?.NeedsToWalk() ?? Input.GetButton("Fire1"))
        {
            jointMotor.motorSpeed = NinjaDir == Dir.Left ? -_speed : _speed;
            hinge.motor = jointMotor;
            hinge.anchor = GetContactPosition();

            yield return null;
        }

        WallJoint.useMotor = false;
        _walkOnWalls = null;
    }

}
