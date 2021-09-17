using System.Collections;
using UnityEngine;

public class Stickiness : MonoBehaviour, IDynamic
{
    [SerializeField] protected float _speed;
    protected Rigidbody2D _rigidbody;
    protected Collider2D _collider;

    public HingeJoint2D WallJoint { get; set; }
    public Obstacle CurrentAttachment { get; set; }
    public bool Attached { get; private set; }
    public bool Active { get; set; }
    public bool CanWalk { get; set; }
    public bool Walking => _walkOnWalls != null;
    public float CurrentSpeed { get; set; }
    public Transform Transform => _transform;
    public Transform ContactPoint { get; private set; }
    public Rigidbody2D Rigidbody { get { if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody2D>(); return _rigidbody; } }
    public Collider2D Collider { get { if (_collider == null) _collider = GetComponent<Collider2D>(); return _collider; } }
    public Vector3 CollisionNormal { get; private set; }
    public float ImpactVelocity { get; private set; }
    public bool DynamicActive => true;
    public bool Running { get; protected set; }
    public PoolableType PoolableType => PoolableType.None;

    protected float _speedFactor;
    protected Vector3 _previousContactPoint;
    protected Coroutine _walkOnWalls;
    protected Jumper _jumper;
    protected Transform _transform;
    private float _velocityBeforePhysicsUpdate;
    private float _detachTime;
    private Vector2 _detachPos;

    public virtual void Awake()
    {
        if (Active)
            return;

        Active = true;
        _transform = transform;
        WallJoint = GetComponent<HingeJoint2D>();
        _jumper = GetComponent<Jumper>();
        _rigidbody = _rigidbody ?? GetComponent<Rigidbody2D>();
        _collider = _collider ?? GetComponent<Collider2D>();
        ContactPoint = new GameObject("ContactPoint").transform;
        ContactPoint.position = _transform.position;
        ContactPoint.SetParent(_transform);
        _previousContactPoint = ContactPoint.position;
    }

    public virtual void Start()
    {
        if (CanWalk)
            return;

        CanWalk = true;
        ReinitSpeed();
    }

    private void FixedUpdate()
    {
        _velocityBeforePhysicsUpdate = Rigidbody.velocity.magnitude;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ImpactVelocity = _velocityBeforePhysicsUpdate;
        OnCollisionStay2D(collision);
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        var contact = GetContactPoint(collision.contacts, _previousContactPoint);
        SetContactPosition(contact.point);
        CollisionNormal = Quaternion.Euler(0, 0, -90) * contact.normal;
    }

    public virtual bool Attach(Obstacle obstacle)
    {
        if (Attached)
            return false;

        var dynamic = obstacle as DynamicObstacle;

        if (dynamic != null && !dynamic.DynamicActive)
            return false;

        var anchorPos = _transform.InverseTransformPoint(GetContactPosition());
        var deltaTime = Time.time - _detachTime;
        var deltaPos = (_detachPos - new Vector2(anchorPos.x, anchorPos.y)).magnitude;

        // Threshold to attach
        if (deltaTime < .05f && deltaPos < .6f)
            return false;

        WallJoint.enabled = true;
        WallJoint.useMotor = false;
        WallJoint.anchor = anchorPos;
        WallJoint.connectedAnchor = WallJoint.anchor;

        Attached = true;

        Rigidbody.gravityScale = 0;
        Rigidbody.velocity = Vector2.zero;

        return true;
    }

    public virtual void Detach()
    {
        if (!Attached)
            return;

        _detachTime = Time.time;
        _detachPos = WallJoint.anchor;
        Rigidbody.gravityScale = 1;
        WallJoint.enabled = false;
        CurrentAttachment = null;
        Attached = false;
    }

    public virtual bool ReactToObstacle(Obstacle obstacle, Vector3 contactPoint)
    {
        if (!Active || obstacle == CurrentAttachment)
            return false;

        if (obstacle.CompareTag("Wall") || obstacle.CompareTag("DynamicWall") || obstacle.CompareTag("TutorialWall"))
        {
            Detach();

            if (!Attach(obstacle))
                return false;

            CurrentAttachment = obstacle;
            CurrentAttachment = obstacle;
            SetContactPosition(contactPoint);
            return true;
        }

        return false;
    }

    public ContactPoint2D GetContactPoint(ContactPoint2D[] contacts, Vector3 previousPos) //WOOOOHOOO ça marche !!!!!
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

    public Vector3 GetContactPosition()
    {
        return ContactPoint.position;
    }

    public void SetContactPosition(Vector3 position)
    {
        _previousContactPoint = ContactPoint.position;
        ContactPoint.position = position;
    }

    public virtual void StopWalking(bool stayGrounded)
    {
        if (_walkOnWalls != null)
        {
            StopCoroutine(_walkOnWalls);
            _walkOnWalls = null;
        }

        if (!Attached)
            return;

        _speedFactor = 0;
        Rigidbody.velocity = Vector2.zero;
        Rigidbody.angularVelocity = 0;
        WallJoint.useMotor = false;
        Rigidbody.isKinematic = stayGrounded;

    }

    public virtual void StartWalking()
    {
        if (_walkOnWalls != null || !Active)
            return;

        Rigidbody.isKinematic = false;
        _walkOnWalls = StartCoroutine(WalkOnWalls(WallJoint));
    }

    protected virtual IEnumerator WalkOnWalls(HingeJoint2D hinge)
    {
        if (hinge == null)
            yield break;

        var jointMotor = hinge.motor;
        hinge.useMotor = true;

        while (true)
        {
            jointMotor.motorSpeed = CurrentSpeed;
            hinge.motor = jointMotor;
            hinge.anchor = _transform.InverseTransformPoint(GetContactPosition());

            yield return null;
        }
    }

    public void StartRunning()
    {
        Running = true;
        CurrentSpeed *= 2;
    }

    public void ReinitSpeed()
    {
        Running = false;
        CurrentSpeed = _speed;
    }

    public void LaunchQuickDeactivate()
    {
        Detach();
    }
}
