using System.Collections;
using UnityEngine;

public class PatrollingEnemy : EnemyNinja, ITriggerable
{
    private Coroutine _trigger;

    public bool Triggered { get; private set; }
    public int LastTrigger { get; private set; }

    protected override void Start()
    {
        base.Start();
        SetAttacking(true);
        Stickiness.StartWalking();
    }


    public IEnumerator Trigger(EventTrigger trigger)
    {
        Triggered = true;
        LastTrigger = trigger.Id;
        Stickiness.CurrentSpeed *= -1; 

        yield return new WaitForSeconds(1);

        Triggered = false;

        _trigger = null;
    }

    public bool IsTriggeredBy(int triggerId)
    {
        return LastTrigger == triggerId;
    }

    public void StartTrigger(EventTrigger trigger)
    {
        if (_trigger == null)
        {
            _trigger = StartCoroutine(Trigger(trigger));
        }
    }
}
