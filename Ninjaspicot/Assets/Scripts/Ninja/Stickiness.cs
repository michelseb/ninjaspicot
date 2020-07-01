using System.Collections;
using UnityEngine;

public enum Dir
{
    Left,
    Right
}

public class Stickiness : MonoBehaviour, IDynamic
{
    [SerializeField] private float _speed;
    protected Rigidbody2D _rigidbody;

    public HingeJoint2D WallJoint { get; set; }
    public Obstacle CurrentAttachment { get; set; }
    public bool Attached { get; private set; }
    public bool Active { get; set; }
    public bool CanWalk { get; set; }
    public bool Walking => _walkOnWalls != null;
    public Dir NinjaDir { get; set; }
    public float CurrentSpeed { get; set; }
    public Transform ContactPoint { get; private set; }
    public Rigidbody2D Rigidbody { get { if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody2D>(); return _rigidbody; } }
    public bool DynamicActive => true;
    public PoolableType PoolableType => PoolableType.None;

    protected INinja _ninjaBehaviour;
    protected Vector3 _previousContactPoint;
    protected Coroutine _walkOnWalls;
    protected Jumper _jumpManager;

    public virtual void Awake()
    {
        if (Active)
            return;

        Active = true;
        WallJoint = GetComponent<HingeJoint2D>();
        _jumpManager = GetComponent<Jumper>();
        _ninjaBehaviour = GetComponent<INinja>();
        ContactPoint = new GameObject("ContactPoint").transform;
        ContactPoint.position = transform.position;
        ContactPoint.SetParent(transform);
        _previousContactPoint = ContactPoint.position;
    }

    public virtual void Start()
    {
        if (CanWalk)
            return;

        CanWalk = true;
        CurrentSpeed = _speed;
    }

    protected virtual void Update()
    {
        if (Walking && !_ninjaBehaviour.ReadyToWalk)
        {
            StopWalking(true);
        }
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    SetContactPosition(GetContactPoint(collision.contacts, _previousContactPoint), true);
    //}

    private void OnCollisionStay2D(Collision2D collision)
    {
        SetContactPosition(GetContactPoint(collision.contacts, _previousContactPoint));
    }


    public virtual void Attach(Obstacle obstacle)
    {
        var dynamic = obstacle as DynamicObstacle;
        if (dynamic != null && !dynamic.DynamicActive)
            return;

        WallJoint.enabled = true;
        WallJoint.useMotor = false;
        WallJoint.anchor = transform.InverseTransformPoint(GetContactPosition());
        WallJoint.connectedAnchor = WallJoint.anchor;

        Attached = true;

        Rigidbody.gravityScale = 0;
        Rigidbody.velocity = Vector2.zero;
    }

    public virtual void Detach()
    {
        if (CurrentAttachment != null && CurrentAttachment is DynamicObstacle)
        {
            ((DynamicObstacle)CurrentAttachment).LaunchQuickDeactivate();
        }
        Rigidbody.gravityScale = 1;
        WallJoint.enabled = false;
        CurrentAttachment = null;
        Attached = false;
    }

    public virtual void ReactToObstacle(Obstacle obstacle, Vector3 contactPoint)
    {
        if (!Active || obstacle == CurrentAttachment)
            return;

        if (obstacle.CompareTag("Wall") || obstacle.CompareTag("DynamicWall") || obstacle.CompareTag("TutorialWall"))
        {
            Detach();
            Attach(obstacle);
            CurrentAttachment = obstacle;
            SetContactPosition(contactPoint);
        }
    }

    public Vector3 GetContactPoint(ContactPoint2D[] contacts, Vector3 previousPos) //WOOOOHOOO ça marche !!!!!
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

        return resultContact.point;
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
        if (!Attached)
            return;

        if (_walkOnWalls != null)
        {
            StopCoroutine(_walkOnWalls);
        }

        Rigidbody.velocity = Vector2.zero;
        Rigidbody.angularVelocity = 0;
        WallJoint.useMotor = false;

        if (stayGrounded)
        {
            //Rigidbody.isKinematic = true;
        }

        _walkOnWalls = null;
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
            jointMotor.motorSpeed = NinjaDir == Dir.Left ? -CurrentSpeed : CurrentSpeed;
            hinge.motor = jointMotor;
            hinge.anchor = transform.InverseTransformPoint(GetContactPosition());

            yield return null;
        }
    }

    public void ReinitSpeed()
    {
        CurrentSpeed = _speed;
    }

}
