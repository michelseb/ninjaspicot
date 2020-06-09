using System.Collections;
using UnityEngine;

public enum Dir
{
    Left,
    Right
}

public class Stickiness : MonoBehaviour
{
    [SerializeField] private int _speed;

    public HingeJoint2D WallJoint { get; set; }
    public Transform CurrentAttachment { get; set; }
    public bool Attached { get; private set; }
    public bool Active { get; set; }
    public bool CanWalk { get; set; }
    private IDynamic _dynamicEntity;
    private ContactPoint2D _contactPoint;
    private ContactPoint2D _previousContactPoint;
    private Coroutine _walkOnWalls;
    private Dir _ninjaDir;
    private TouchManager _touchManager;
    private JumpManager _jumpManager;

    private void Awake()
    {
        WallJoint = GetComponent<HingeJoint2D>();
        _touchManager = TouchManager.Instance;
        _dynamicEntity = GetComponent<IDynamic>();
        _jumpManager = GetComponent<JumpManager>();
    }

    private void Start()
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
        _contactPoint = GetContactPoint(collision.contacts, _previousContactPoint);
        _previousContactPoint = _contactPoint;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        _contactPoint = GetContactPoint(collision.contacts, _previousContactPoint);
        _previousContactPoint = _contactPoint;
    }


    public void Attach()
    {
        WallJoint.enabled = true;
        WallJoint.useMotor = false;
        WallJoint.anchor = transform.InverseTransformPoint(_contactPoint.point);
        WallJoint.connectedAnchor = WallJoint.anchor;

        Attached = true;

        _dynamicEntity.Rigidbody.gravityScale = 0;
        _dynamicEntity.Rigidbody.velocity = Vector2.zero;
    }

    public void Detach()
    {
        _dynamicEntity.Rigidbody.gravityScale = 1;
        WallJoint.enabled = false;
        CurrentAttachment = null;
        Attached = false;
    }

    public void ReactToObstacle(Transform obstacle)
    {
        if (!Active || obstacle == CurrentAttachment)
            return;

        _jumpManager.GainAllJumps();

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

    public Vector2 GetContactPosition()
    {
        return _contactPoint.point;
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

    public IEnumerator WalkOnWalls(HingeJoint2D hinge)
    {
        if (hinge == null)
            yield break;

        var jointMotor = hinge.motor;
        WallJoint.useMotor = true;

        while (Input.GetButton("Fire1"))
        {

            if (_ninjaDir == Dir.Right)
            {
                jointMotor.motorSpeed = _speed;
            }
            else if (_ninjaDir == Dir.Left)
            {
                jointMotor.motorSpeed = -_speed;
            }
            hinge.motor = jointMotor;
            hinge.anchor = transform.InverseTransformPoint(GetContactPosition());
            hinge.connectedAnchor = hinge.anchor;

            yield return null;
        }


        WallJoint.useMotor = false;
        _walkOnWalls = null;
    }

}
