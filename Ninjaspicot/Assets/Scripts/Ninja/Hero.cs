﻿using System.Collections;
using UnityEngine;

public class Hero : Ninja, IRaycastable
{
    public bool Triggered { get; private set; }
    public DynamicInteraction DynamicInteraction { get; private set; }

    private int _lastTrigger;
    private Cloth _cape;
    private TimeManager _timeManager;
    private SpawnManager _spawnManager;
    private TouchManager _touchManager;

    private static Hero _instance;
    public static Hero Instance { get { if (_instance == null) _instance = FindObjectOfType<Hero>(); return _instance; } }


    protected override void Awake()
    {
        base.Awake();
        _timeManager = TimeManager.Instance;
        _spawnManager = SpawnManager.Instance;
        _cameraBehaviour = CameraBehaviour.Instance;
        _touchManager = TouchManager.Instance;
        _cape = GetComponentInChildren<Cloth>();
        DynamicInteraction = GetComponent<DynamicInteraction>() ?? GetComponentInChildren<DynamicInteraction>();

    }

    public override void Die(Transform killer)
    {
        if (Dead)
            return;

        base.Die(killer);
        Renderer.color = ColorUtils.Red;
        _timeManager.StartSlowDownProgressive(.3f);

        if (DynamicInteraction.Interacting)
        {
            DynamicInteraction.StopInteraction(false);
        }
    }

    public override IEnumerator Dying()
    {
        yield return new WaitForSeconds(1);

        SetCapeActivation(false);
        _spawnManager.Respawn();
        SetAllBehavioursActivation(true, false);
        _cameraBehaviour.SetCenterMode(transform, 1f);
        SetCapeActivation(true);

        yield return new WaitForSeconds(1f);

        _cameraBehaviour.SetFollowMode(transform);
    }

    public void SetCapeActivation(bool active)
    {
        _cape.GetComponent<SkinnedMeshRenderer>().enabled = active;
        _cape.enabled = active;
    }

    public IEnumerator Trigger(EventTrigger trigger)
    {
        Triggered = true;
        _lastTrigger = trigger.Id;
        
        yield return new WaitForSeconds(3);
        
        Triggered = false;

        if (trigger.SingleTime)
        {
            Destroy(trigger.gameObject);
        }
    }

    public bool IsTriggeredBy(int id)
    {
        return _lastTrigger == id;
    }

    public void SetInteractionActivation(bool active)
    {
        DynamicInteraction.Active = active;
    }

    public override void SetAllBehavioursActivation(bool active, bool grounded)
    {
        base.SetAllBehavioursActivation(active, grounded);
        SetInteractionActivation(active);
    }

    public override bool NeedsToWalk()
    {
        return _touchManager.Touching;
    }
}
