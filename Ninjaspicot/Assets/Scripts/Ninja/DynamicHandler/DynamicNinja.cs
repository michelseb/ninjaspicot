﻿using UnityEngine;

public class DynamicNinja : MonoBehaviour, INinja
{
    private Ninja _linkedNinja;
    public Jumper JumpManager { get; private set; }
    public DynamicStickiness Stickiness { get; private set; }
    public bool ReadyToWalk => NeedsToWalk();

    protected virtual void Awake()
    {
        JumpManager = GetComponent<Jumper>();
        Stickiness = GetComponent<DynamicStickiness>();
    }

    protected virtual void Update()
    {
        if (Stickiness.Walking && !NeedsToWalk())
        {
            Stickiness.StopWalking(true);
        }
    }

    public virtual bool NeedsToWalk()
    {
        return _linkedNinja.NeedsToWalk();
    }

    public void SetNinja(Ninja ninja)
    {
        _linkedNinja = ninja;
    }
}
