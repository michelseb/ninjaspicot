using UnityEngine;

public class Stickiness : MonoBehaviour
{
    public HingeJoint2D WallJoint { get; set; }
    public Transform CurrentAttachment { get; set; }
    public bool Attached { get; private set; }
    public bool Active { get; set; }

    private DynamicEntity _dynamicEntity;
    private ContactPoint2D _contactPoint;
    private ContactPoint2D _previousContactPoint;

    protected virtual void Awake()
    {
        WallJoint = GetComponent<HingeJoint2D>();
        _dynamicEntity = GetComponent<DynamicEntity>();
    }

    protected virtual void Start()
    {
        Active = true;
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

    public void Detach() // SET WALKING TO FALSE EVERYWHERE
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

}
