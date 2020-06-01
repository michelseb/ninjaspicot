﻿using UnityEngine;

[DisallowMultipleComponent]
public class Wall : DynamicEntity
{
    public bool IsBeingTouched { get; private set; }

    private GameObject _contact;
    private ContactPoint2D _contactPoint;

    private Ninja _ninjaTemp;

    private void Start()
    {
        _contact = new GameObject();
        _contact.transform.position = transform.position;
        _contact.transform.parent = transform;

    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        var ninja = collision.gameObject.GetComponent<Ninja>();

        if (ninja == null)
            return;

        _ninjaTemp = ninja;

        ninja.Movement.GainAllJumps();
        ninja.Stickiness.ReactToCollider(Rigidbody);

        _contactPoint = collision.contacts[collision.contacts.Length - 1];
        if (ninja.Stickiness.CurrentAttachment == null)
        {
            ninja.Stickiness.CurrentAttachment = transform;
        }

        if (ninja.Stickiness.CheckRecentCollider(transform))
        {
            ninja.Stickiness.SetContactPoint(collision.contacts[collision.contacts.Length - 1]);
            ninja.Stickiness.CurrentAttachment = transform;
        }

    }


    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (_ninjaTemp == null)
            return;

        IsBeingTouched = true;
        _contactPoint = collision.contacts[collision.contacts.Length - 1];
        _ninjaTemp.Stickiness.AddToColliders(transform);
        PositionContact(_contactPoint.point);
    }

    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ninja")
        {
            IsBeingTouched = false;
        }
    }

    public void PositionContact(Vector3 pos)
    {
        _contact.transform.position = pos;
    }

    public Vector3 GetContactPoint()
    {
        return _contactPoint.point;
    }
}
