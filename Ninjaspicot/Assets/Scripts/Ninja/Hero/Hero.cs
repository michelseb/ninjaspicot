using System.Collections;
using UnityEngine;

public class Hero : Ninja, IRaycastable, ITriggerable
{
    [SerializeField] float _ghostSpacing;
    public bool Triggered { get; private set; }
    public DynamicInteraction DynamicInteraction { get; private set; }
    public int LastTrigger { get; private set; }

    private Cloth _cape;
    private TimeManager _timeManager;
    private SpawnManager _spawnManager;
    private TouchManager _touchManager;
    private Coroutine _displayGhosts;
    private Coroutine _fade;
    private Coroutine _trigger;
    private static Hero _instance;
    public static Hero Instance { get { if (_instance == null) _instance = FindObjectOfType<Hero>(); return _instance; } }

    private const float FADE_SPEED = 5f;

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
        StopDisplayGhosts();
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
        _cameraBehaviour.SetCenterMode(Transform, 1f);
        SetCapeActivation(true);

        yield return new WaitForSeconds(1f);

        _cameraBehaviour.SetFollowMode(Transform);
    }

    public void SetCapeActivation(bool active)
    {
        _cape.GetComponent<SkinnedMeshRenderer>().enabled = active;
        _cape.enabled = active;
    }

    public IEnumerator Trigger(EventTrigger trigger)
    {
        Triggered = true;
        LastTrigger = trigger.Id;
        
        yield return new WaitForSeconds(3);
        
        Triggered = false;

        if (trigger.SingleTime)
        {
            Destroy(trigger.gameObject);
        }
        _trigger = null;
    }

    public bool IsTriggeredBy(int id)
    {
        return LastTrigger == id;
    }

    public void SetInteractionActivation(bool active)
    {
        if (active)
        {
            DynamicInteraction.Activate();
        }
        else
        {
            DynamicInteraction.Deactivate();
        }
    }

    public override void SetAllBehavioursActivation(bool active, bool grounded)
    {
        base.SetAllBehavioursActivation(active, grounded);
        SetInteractionActivation(active);
    }

    public override bool NeedsToWalk()
    {
        return _touchManager.WalkDragging;
    }

    public void StartDisplayGhosts()
    {
        if (_displayGhosts != null)
            return;

        _displayGhosts = StartCoroutine(DisplayGhosts(_ghostSpacing));
    }

    public void StopDisplayGhosts()
    {
        if (_displayGhosts == null)
            return;

        StopCoroutine(_displayGhosts);
        _displayGhosts = null;
    }

    private IEnumerator DisplayGhosts(float delay)
    {
        while (true)
        {
            _poolManager.GetPoolable<HeroGhost>(Transform.position, Transform.rotation);
            yield return new WaitForSeconds(delay);
        }
    }

    public void StartFading()
    {
        _fade = StartCoroutine(FadeAway());
    }

    public void StartAppear()
    {
        if (_fade != null)
        {
            StopCoroutine(_fade);
        }

        StartCoroutine(Appear());
    }


    private IEnumerator FadeAway()
    {
        Color col = Renderer.color;
        while (col.a > 0)
        {
            col = Renderer.color;
            col.a -= Time.deltaTime * FADE_SPEED;
            Renderer.color = col;
            yield return null;
        }
        Debug.Log("Vector0");
        Stickiness.Rigidbody.velocity = Vector2.zero;
        Debug.Log("Kinematic");
        Stickiness.Rigidbody.isKinematic = true;
        _fade = null;
    }

    private IEnumerator Appear()
    {
        Color col = Renderer.color;
        while (col.a < 1)
        {
            col = Renderer.color;
            col.a += Time.deltaTime * FADE_SPEED;
            Renderer.color = col;
            yield return null;
        }
    }

    public void StartTrigger(EventTrigger trigger)
    {
        if (_trigger == null)
        {
            _trigger = StartCoroutine(Trigger(trigger));
        }
    }
}
