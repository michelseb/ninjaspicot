using System.Collections;
using UnityEngine;

public class Hero : Character, INinja, ITriggerable
{
    [SerializeField] float _ghostSpacing;
    public bool Triggered { get; private set; }
    public DynamicInteraction DynamicInteraction { get; private set; }
    public int LastTrigger { get; private set; }
    public bool Detected { get; private set; }
    public bool Visible { get; private set; }

    private HeroStickiness _stickiness;
    public Stickiness Stickiness { get { if (Utils.IsNull(_stickiness)) _stickiness = GetComponent<HeroStickiness>(); return _stickiness; } }

    private GrapplingGun _grapplingGun;
    public GrapplingGun GrapplingGun { get { if (Utils.IsNull(_grapplingGun)) _grapplingGun = GetComponent<GrapplingGun>(); return _grapplingGun; } }
    
    public Rigidbody2D Rigidbody => Stickiness.Rigidbody;

    private Cloth _cape;
    private TimeManager _timeManager;
    private SpawnManager _spawnManager;
    private TouchManager _touchManager;
    private ZoneManager _zoneManager;
    private Coroutine _displayGhosts;
    private Coroutine _fade;
    private Coroutine _trigger;
    private static Hero _instance;
    private UICamera _uiCamera;
    private AudioSource _mainAudioSource;
    public static Hero Instance { get { if (_instance == null) _instance = FindObjectOfType<Hero>(); return _instance; } }

    private const int GHOST_SOUND_FREQUENCY = 3;
    private const float FADE_SPEED = 5f;

    protected override void Awake()
    {
        base.Awake();
        _mainAudioSource = ScenesManager.Instance.GetComponent<AudioSource>();
        _timeManager = TimeManager.Instance;
        _spawnManager = SpawnManager.Instance;
        _cameraBehaviour = CameraBehaviour.Instance;
        _touchManager = TouchManager.Instance;
        _zoneManager = ZoneManager.Instance;
        _uiCamera = UICamera.Instance;
        _grapplingGun = GetComponentInChildren<GrapplingGun>(true);
        _cape = GetComponentInChildren<Cloth>();
        DynamicInteraction = GetComponent<DynamicInteraction>() ?? GetComponentInChildren<DynamicInteraction>();
        Visible = true;
    }

    public override void Die(Transform killer = null, Audio sound = null, float volume = 1f)
    {
        if (Dead)
            return;

        Dead = true;
        Stickiness.Detach();
        _grapplingGun?.Cancel();
        
        if (killer != null)
        {
            Stickiness.Rigidbody.AddForce((Transform.position - killer.position).normalized * 10000);
        }

        if (sound != null)
        {
            _audioManager.PlaySound(_audioSource, sound, volume);
        }
        
        SetAllBehavioursActivation(false, false);
        StopDisplayGhosts();
        Image.color = ColorUtils.Red;
        _timeManager.StartSlowDownProgressive(.3f);

        if (DynamicInteraction.Interacting)
        {
            DynamicInteraction.StopInteraction(false);
        }



        StartCoroutine(Dying());
    }

    public override IEnumerator Dying()
    {
        _mainAudioSource.Stop();

        yield return new WaitForSecondsRealtime(.5f);

        _uiCamera.CameraFade();

        yield return new WaitForSecondsRealtime(1.5f);

        _zoneManager.CurrentZone.ResetItems();
        _timeManager.SetNormalTime();
        SetCapeActivation(false);
        _spawnManager.Respawn();
        SetAllBehavioursActivation(true, false);
        SetCapeActivation(true);
        _uiCamera.CameraAppear();
        _mainAudioSource.Play();
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

    public void StartDisplayGhosts(bool silent, float? spacing = null)
    {
        if (_displayGhosts != null)
            return;

        _displayGhosts = StartCoroutine(DisplayGhosts(silent, spacing ?? _ghostSpacing));
    }

    public void StopDisplayGhosts()
    {
        if (_displayGhosts == null)
            return;

        StopCoroutine(_displayGhosts);
        _displayGhosts = null;
    }

    private IEnumerator DisplayGhosts(bool silent, float delay)
    {
        int iteration = 0;
        while (true)
        {
            _poolManager.GetPoolable<HeroGhost>(Transform.position, Transform.rotation);
            if (!silent && iteration % GHOST_SOUND_FREQUENCY == 0)
            {
                _poolManager.GetPoolable<SoundEffect>(Transform.position, Quaternion.identity, 2);
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
        Color col = Image.color;
        while (col.a > 0)
        {
            col = Image.color;
            col.a -= Time.deltaTime * FADE_SPEED;
            Image.color = col;
            yield return null;
        }
        Stickiness.Rigidbody.velocity = Vector2.zero;
        Stickiness.Rigidbody.isKinematic = true;
        _fade = null;
    }

    private IEnumerator Appear()
    {
        Color col = Image.color;
        while (col.a < 1)
        {
            col = Image.color;
            col.a += Time.deltaTime * FADE_SPEED;
            Image.color = col;
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

    public void SetGrapplingActivation(bool active)
    {
        if (!Utils.IsNull(_grapplingGun))
        {
            _grapplingGun.Active = active;
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
        SetGrapplingActivation(active);
        SetStickinessActivation(active);
        SetWalkingActivation(active, grounded);
        SetInteractionActivation(active);
    }

    public virtual bool NeedsToWalk()
    {
        return _touchManager.WalkDragging;
    }

    public virtual void Hide()
    {
        Visible = false;
    }

    public virtual void Reveal()
    {
        Visible = true;
    }

    public virtual void ActivateGrappling()
    {
        _grapplingGun.Show();
    }

    public virtual void ThrowGrappling(IFocusable target)
    {
        if (target == null)
            return;

        _grapplingGun.StartThrow(target);
    }

    public virtual void DeactivateGrappling()
    {
        _grapplingGun.Hide();
    }

    //public virtual void ActivateCollider()
    //{
    //    Collider.enabled = true;
    //}

    //public virtual void DeactivateCollider()
    //{
    //    Collider.enabled = false;
    //}
}
