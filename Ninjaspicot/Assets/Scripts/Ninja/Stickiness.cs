using System.Collections.Generic;
using UnityEngine;

public class Stickiness : MonoBehaviour
{
    public HingeJoint2D WallJoint { get; set; }
    public Transform CurrentAttachment { get; set; }
    public bool Attached { get; private set; }
    public bool Sticking { get; private set; }

    private DynamicEntity _dynamicEntity;
    private HingeJoint2D _dynamicJoint;
    private FixedJoint2D _fixedJoint;
    private ContactPoint2D _contactPoint;
    private ContactPoint2D _previousContactPoint;

    private Queue<Transform> _lastColliders;

    protected virtual void Awake()
    {
        WallJoint = GetComponent<HingeJoint2D>();
        _lastColliders = new Queue<Transform>();
        _dynamicEntity = GetComponent<DynamicEntity>();
    }

    protected virtual void Update()
    {
        if (_lastColliders.Count > 5)
        {
            _lastColliders.Dequeue();
        }
    }


    public void OnCollisionStay2D(Collision2D collision)
    {
        _contactPoint = GetContactPoint(collision.contacts, _previousContactPoint);
        _previousContactPoint = _contactPoint;
    }


    public void Attach(Rigidbody2D otherBody)
    {

        if (otherBody.gameObject.GetComponent<StickyBody>())
        {
            if (_fixedJoint == null)
            {
                _fixedJoint = gameObject.AddComponent<FixedJoint2D>();
                _fixedJoint.anchor = transform.InverseTransformPoint(_contactPoint.point);
                _fixedJoint.connectedAnchor = _fixedJoint.anchor;
                _fixedJoint.enableCollision = true;
                _fixedJoint.connectedBody = _dynamicEntity.Rigidbody;
            }
        }
        _dynamicEntity.Rigidbody.velocity = new Vector2(0, 0);
        _dynamicJoint = gameObject.AddComponent<HingeJoint2D>();
        _dynamicJoint.useLimits = false;
        _dynamicJoint.anchor = transform.InverseTransformPoint(_contactPoint.point);
        _dynamicJoint.connectedAnchor = _dynamicJoint.anchor;
        _dynamicJoint.enableCollision = true;
        _dynamicJoint.connectedBody = otherBody;
        _dynamicJoint.useMotor = true;
        JointMotor2D motor = _dynamicJoint.motor;
        _dynamicJoint.motor = motor;
        Sticking = true;
        WallJoint = _dynamicJoint;
        Debug.Log("Attached");

        Attached = true;

    }

    public void Detach() // SET WALKING TO FALSE EVERYWHERE
    {
        if (!Attached)
            return;

        if (_dynamicJoint != null)
        {
            Destroy(_dynamicJoint);
        }
        if (_fixedJoint != null)
        {
            Destroy(_fixedJoint);
        }

        Attached = false;
        Debug.Log("Detached");
    }

    public void ReactToCollider(Rigidbody2D rigidbody)
    {
        if (rigidbody == null)
            return;

        if (rigidbody.CompareTag("Wall") && rigidbody != CurrentAttachment)
        {
            Detach();
            Attach(rigidbody);
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

    public bool CheckRecentCollider(Transform col)
    {
        if (col != CurrentAttachment)
        {
            if (_lastColliders.Contains(col))
                return false;
        }

        return true;
    }

    public bool HasBeenCurrentCollider(Transform col)
    {
        if (col != CurrentAttachment)
        {
            if (!_lastColliders.Contains(col))
                return false;
        }
        return true;
    }


    public void AddToColliders(Transform newCol)
    {
        _lastColliders.Enqueue(newCol);
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
