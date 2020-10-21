using System.Collections;
using UnityEngine;

public class Hero : Character, INinja, IRaycastable, ITriggerable
{
    [SerializeField] float _ghostSpacing;
    public bool Triggered { get; private set; }
    public DynamicInteraction DynamicInteraction { get; private set; }
    public int LastTrigger { get; private set; }
    public bool Detected { get; private set; }
    public bool Visible { get; private set; }

    private HeroJumper _jumper;
    public Jumper Jumper { get { if (Utils.IsNull(_jumper)) _jumper = GetComponent<HeroJumper>(); return _jumper; } }

    private HeroStickiness _stickiness;
    public Stickiness Stickiness { get { if (Utils.IsNull(_stickiness)) _stickiness = GetComponent<HeroStickiness>(); return _stickiness; } }

    private Cloth _cape;
    private TimeManager _timeManager;
    private SpawnManager _spawnManager;
    private TouchManager _touchManager;
    private Coroutine _displayGhosts;
    private Coroutine _fade;
    private Coroutine _trigger;
    private static Hero _instance;
    public static Hero Instance { get { if (_instance == null) _instance = FindObjectOfType<Hero>(); return _instance; } }

    private const int GHOST_SOUND_FREQUENCY = 3;
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

        Dead = true;
        Stickiness.Detach();
        Jumper?.CancelJump();
        SetAllBehavioursActivation(false, false);
        StopDisplayGhosts();
        Renderer.color = ColorUtils.Red;
        _timeManager.StartSlowDownProgressive(.3f);


        if (DynamicInteraction.Interacting)
        {
            DynamicInteraction.StopInteraction(false);
        }

        StartCoroutine(Dying());
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
        int iteration = 0;
        while (true)
        {
            _poolManager.GetPoolable<HeroGhost>(Transform.position, Transform.rotation);
            if (iteration % GHOST_SOUND_FREQUENCY == 0)
            {
                var soundEffect = _poolManager.GetPoolable<SoundEffect>(Transform.position, Quaternion.identity, 2);
                soundEffect.SetComposite(Stickiness.CurrentAttachment.Collider);
            }
            iteration++;
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
        Stickiness.Rigidbody.velocity = Vector2.zero;
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

    public void SetJumpingActivation(bool active)
    {
        if (!Utils.IsNull(Jumper))
        {
            Jumper.Active = active;
        }
    }

    public void SetStickinessActivation(bool active)
    {
        if (!Utils.IsNull(Stickiness))
        {
            Stickiness.Active = active;
        }
    }

    public void SetWalkingActivation(bool active, bool grounded)
    {
        if (Utils.IsNull(Stickiness))
            return;

        if (!active)
        {
            Stickiness.StopWalking(grounded);
        }

        Stickiness.CanWalk = active;
    }

    public virtual void SetAllBehavioursActivation(bool active, bool grounded)
    {
        SetJumpingActivation(active);
        SetStickinessActivation(active);
        SetWalkingActivation(active, grounded);
        SetInteractionActivation(active);
    }

    public virtual bool NeedsToWalk()
    {
        return _touchManager.WalkDragging;
    }
}
